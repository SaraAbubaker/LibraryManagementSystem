using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Services.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IGenericRepository<InventoryRecord> _inventoryRepo;
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly IBookService _bookService;

        public InventoryService(
            IGenericRepository<InventoryRecord> inventoryRepo,
            IGenericRepository<Book> bookRepo,
            IBookService bookService)
        {
            _inventoryRepo = inventoryRepo;
            _bookRepo = bookRepo;
            _bookService = bookService;
        }
        
        //Create, Remove, Read
        public async Task<InventoryRecord> CreateCopyAsync(int bookId, string copyCode, int createdByUserId)
        {
            Validate.Positive(bookId, nameof(bookId));
            Validate.NotEmpty(copyCode, nameof(copyCode));
            Validate.Positive(createdByUserId, nameof(createdByUserId));
            
            var record = new InventoryRecord
            {
                BookId = bookId,
                CopyCode = copyCode,
                IsAvailable = true
            };

            await _inventoryRepo.AddAsync(record, createdByUserId);
            await _inventoryRepo.CommitAsync();

            return record;
        }

        public IQueryable<InventoryRecord> ListCopiesForBookQuery(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));

            return _inventoryRepo.GetAll()
                .AsNoTracking()
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.Id);
        }

        public IQueryable<InventoryRecord> GetAvailableCopiesQuery(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));

            return _inventoryRepo.GetAll()
                .AsNoTracking()
                .Where(r => r.BookId == bookId && r.IsAvailable);
        }

        //Available = true + audit fields set
        public async Task<bool> ReturnCopyAsync(int inventoryRecordId, int currentUserId)
        {
            Validate.Positive(inventoryRecordId, nameof(inventoryRecordId));
            Validate.Positive(currentUserId, nameof(currentUserId));

            var copy = Validate.Exists(
                await _inventoryRepo.GetById(inventoryRecordId).FirstOrDefaultAsync(),
                inventoryRecordId
            );

            copy.IsAvailable = true;
            await _inventoryRepo.UpdateAsync(copy, currentUserId);

            return true;
        }

        //Archive
        public async Task<bool> ArchiveCopyAsync(int inventoryRecordId, int performedByUserId)
        {
            Validate.Positive(inventoryRecordId, nameof(inventoryRecordId));
            Validate.Positive(performedByUserId, nameof(performedByUserId));

            var copy = Validate.Exists(
                await _inventoryRepo.GetById(inventoryRecordId).FirstOrDefaultAsync(),
                inventoryRecordId
            );

            await _inventoryRepo.ArchiveAsync(copy, performedByUserId);

            //Archive the book if no copies remain
            var anyLeft = _inventoryRepo.GetAll()
                .Any(r => r.BookId == copy.BookId);
            if (!anyLeft)
            {
                await _bookService.ArchiveBookAsync(copy.BookId, performedByUserId);
            }

            return true;
        }

    }
}
