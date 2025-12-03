using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.BorrowRecord;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly IGenericRepository<BorrowRecord> _borrowRepo;
        private readonly IGenericRepository<InventoryRecord> _inventoryRepo;
        private readonly InventoryService Inventory;

        public BorrowService(
            IGenericRepository<BorrowRecord> borrowRepo,
            IGenericRepository<InventoryRecord> inventoryRepo,
            InventoryService inventoryService)
        {
            _borrowRepo = borrowRepo;
            _inventoryRepo = inventoryRepo;
            Inventory = inventoryService;
        }


        //ListAll
        public List<BorrowDto> GetBorrowDetails()
        {
            var query = _borrowRepo.GetAll()
                .Select(b => new BorrowDto
                {
                    Id = b.Id,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    CopyCode = b.InventoryRecord?.CopyCode,
                    Username = b.User?.Username,
                    IsOverdue = IsBorrowOverdue(b),
                    OverdueDays = CalculateOverdueDays(b)
                });

            return query.ToList();
        }

        //Availability
        public bool HasAvailableCopy(int bookId)
        {
            return Inventory.GetAvailableCopies(bookId).Any();
        }

        public InventoryRecord? GetAvailableCopy(int bookId)
        {
            return Inventory
                .GetAvailableCopies(bookId)
                .FirstOrDefault();
        }

        //Borrow & Return
        public BorrowRecord BorrowBook(RequestBorrowDto dto, int userId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.Positive(dto.InventoryRecordId, nameof(dto.InventoryRecordId));
            Validate.Positive(userId, nameof(userId));

            var copy = _inventoryRepo.GetById(dto.InventoryRecordId);

            copy!.IsAvailable = false;
            _inventoryRepo.Update(copy);

            var borrow = new BorrowRecord
            {
                InventoryRecordId = dto.InventoryRecordId,
                UserId = userId,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = dto.DueDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                ReturnDate = null
            };

            _borrowRepo.Add(borrow);

            return borrow;
        }

        public bool ReturnBook(int borrowRecordId, int currentUserId)
        {
            Validate.Positive(borrowRecordId, nameof(borrowRecordId));
            Validate.Positive(currentUserId, nameof(currentUserId));

            var record = _borrowRepo.GetById(borrowRecordId);

            if (record!.ReturnDate != null)
                throw new ConflictException($"Borrow record with id {borrowRecordId} has already been returned.");

            record.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            record.LastModifiedByUserId = currentUserId;
            record.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            _borrowRepo.Update(record);

            return Inventory.ReturnCopy(record.InventoryRecordId, currentUserId);
        }


        //Overdue Logic
        public List<BorrowRecord> GetOverdueRecords()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return _borrowRepo.GetAll()
                .Where(r => r.ReturnDate == null && r.DueDate < today)
                .ToList();
        }

        public bool IsBorrowOverdue(BorrowRecord record)
        {
            Validate.Exists(record, nameof(record));

            if (record.ReturnDate != null) return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            return today > record.DueDate;
        }

        public int CalculateOverdueDays(BorrowRecord record)
        {
            Validate.Exists(record, nameof(record));

            var endDate = record.ReturnDate ?? DateOnly.FromDateTime(DateTime.Today);
            var dueDate = record.DueDate;

            if (endDate <= dueDate) return 0;

            var days = (endDate.ToDateTime(TimeOnly.MinValue) - dueDate.ToDateTime(TimeOnly.MinValue)).Days;
            return Math.Max(0, days);
        }
    }
}
