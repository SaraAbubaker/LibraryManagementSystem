using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.BorrowRecord;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;

namespace Library.Services.Services
{
    public class BorrowService : IBorrowService
    {
        private readonly IGenericRepository<BorrowRecord> _borrowRepo;
        private readonly IGenericRepository<InventoryRecord> _inventoryRepo;
        private readonly IInventoryService _inventoryService;

        public BorrowService(
            IGenericRepository<BorrowRecord> borrowRepo,
            IGenericRepository<InventoryRecord> inventoryRepo,
            IInventoryService inventoryService) 
        {
            _borrowRepo = borrowRepo;
            _inventoryRepo = inventoryRepo;
            _inventoryService = inventoryService;
        }


        //ListAll
        public async Task<List<BorrowDto>> GetBorrowDetailsAsync()
        {
            var allRecords = await _borrowRepo.GetAllAsync();

            return allRecords.Select(b => new BorrowDto
            {
                Id = b.Id,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                CopyCode = b.InventoryRecord?.CopyCode,
                Username = b.User?.Username,
                IsOverdue = IsBorrowOverdue(b),
                OverdueDays = CalculateOverdueDays(b)
            }).ToList();
        }

        //Availability
        public async Task<bool> HasAvailableCopyAsync(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));
            var copies = await _inventoryService.GetAvailableCopiesAsync(bookId);
            return copies.Any();
        }

        public async Task<InventoryRecord?> GetAvailableCopyAsync(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));
            var copies = await _inventoryService.GetAvailableCopiesAsync(bookId);
            return copies.FirstOrDefault();
        }

        //Borrow & Return
        public async Task<BorrowRecord> BorrowBookAsync(RequestBorrowDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.InventoryRecordId, nameof(dto.InventoryRecordId));
            Validate.Positive(userId, nameof(userId));

            var copy = await _inventoryRepo.GetByIdAsync(dto.InventoryRecordId);
            if (copy == null || !copy.IsAvailable)
                throw new ConflictException("Inventory copy is not available.");

            copy.IsAvailable = false;
            await _inventoryRepo.UpdateAsync(copy);

            var borrow = new BorrowRecord
            {
                InventoryRecordId = dto.InventoryRecordId,
                UserId = userId,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = dto.DueDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                ReturnDate = null,

                CreatedByUserId = userId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                LastModifiedByUserId = userId,
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _borrowRepo.AddAsync(borrow);

            return borrow;
        }

        public async Task<bool> ReturnBookAsync(int borrowRecordId, int currentUserId)
        {
            Validate.Positive(borrowRecordId, nameof(borrowRecordId));
            Validate.Positive(currentUserId, nameof(currentUserId));

            var record = await _borrowRepo.GetByIdAsync(borrowRecordId);
            
            if (record!.ReturnDate != null)
                throw new ConflictException($"Borrow record with id {borrowRecordId} has already been returned.");

            record.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            record.LastModifiedByUserId = currentUserId;
            record.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _borrowRepo.UpdateAsync(record);

            return await _inventoryService.ReturnCopyAsync(record.InventoryRecordId, currentUserId);
        }


        //Overdue Logic
        public async Task<List<BorrowRecord>> GetOverdueRecordsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var allRecords = await _borrowRepo.GetAllAsync();

            return allRecords
                .Where(r => r.ReturnDate == null && r.DueDate < today)
                .ToList();
        }

        public bool IsBorrowOverdue(BorrowRecord record)
        {
            Validate.NotNull(record, nameof(record));

            if (record.ReturnDate != null) return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            return today > record.DueDate;
        }

        public int CalculateOverdueDays(BorrowRecord record)
        {
            Validate.NotNull(record, nameof(record));

            var endDate = record.ReturnDate ?? DateOnly.FromDateTime(DateTime.Today);
            var dueDate = record.DueDate;

            if (endDate <= dueDate) return 0;

            var days = (endDate.ToDateTime(TimeOnly.MinValue) - dueDate.ToDateTime(TimeOnly.MinValue)).Days;
            return Math.Max(0, days);
        }
    }
}
