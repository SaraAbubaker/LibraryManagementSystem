using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class InventoryService
    {
        private readonly IGenericRepository<InventoryRecord> _inventoryRepo;
        private readonly IGenericRepository<Book> _bookRepo;
        private readonly BookService BookService;

        public InventoryService(
            IGenericRepository<InventoryRecord> inventoryRepo,
            IGenericRepository<Book> bookRepo,
            BookService bookService)
        {
            _inventoryRepo = inventoryRepo;
            _bookRepo = bookRepo;
            BookService = bookService;
        }


        //Available = true + audit fields set
        public async Task<bool> ReturnCopyAsync(int inventoryRecordId, int currentUserId)
        {
            Validate.Positive(inventoryRecordId, nameof(inventoryRecordId));
            Validate.Positive(currentUserId, nameof(currentUserId));

            var copy = await _inventoryRepo.GetByIdAsync(inventoryRecordId);

            copy.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateOnly.FromDateTime(DateTime.Today);

            await _inventoryRepo.UpdateAsync(copy);

            return true;
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
                IsAvailable = true,
                IsArchived = false,
                CreatedByUserId = createdByUserId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                LastModifiedByUserId = createdByUserId,
                LastModifiedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            await _inventoryRepo.AddAsync(record);

            return record;
        }

        public async Task<bool> RemoveCopyAsync(int inventoryRecordId, int performedByUserId)
        {
            Validate.Positive(inventoryRecordId, nameof(inventoryRecordId));
            Validate.Positive(performedByUserId, nameof(performedByUserId));

            var copy = await _inventoryRepo.GetByIdAsync(inventoryRecordId);

            copy.IsArchived = true;
            copy.ArchivedByUserId = performedByUserId;
            copy.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            copy.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);
            copy.LastModifiedByUserId = performedByUserId;

            await _inventoryRepo.UpdateAsync(copy);

            //Archive the book if no copies remain
            var anyLeft = (await _inventoryRepo.GetAllAsync())
                .Any(r => r.BookId == copy.BookId && !r.IsArchived);
            if (!anyLeft)
            {
                await BookService.ArchiveBookAsync(copy.BookId, performedByUserId);
            }

            return true;
        }

        public async Task<List<InventoryRecord>> ListCopiesForBookAsync(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));

            var all = await _inventoryRepo.GetAllAsync();
            return all
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public async Task<List<InventoryRecord>> GetAvailableCopiesAsync(int bookId)
        {
            Validate.Positive(bookId, nameof(bookId));

            var all = await _inventoryRepo.GetAllAsync();
            return all
                .Where(r => r.BookId == bookId && r.IsAvailable && !r.IsArchived)
                .ToList();
        }

    }
}
