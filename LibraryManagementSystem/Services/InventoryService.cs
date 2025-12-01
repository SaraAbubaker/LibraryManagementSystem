using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class InventoryService
    {
        private readonly LibraryDataStore Store;
        private readonly BookService BookService;

        public InventoryService(LibraryDataStore store, BookService bookService)
        {
            Store = store;
            BookService = bookService;
        }

        //Available = true + audit fields set
        public bool ReturnCopy(int inventoryRecordId, int currentUserId)
        {
            var copy = Store.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            if (copy == null) return false;

            copy.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateOnly.FromDateTime(DateTime.Today);
            return true;
        }

        //Create, Remove, Read
        public InventoryRecord CreateCopy(int bookId, string copyCode, int createdByUserId)
        {
            if (string.IsNullOrWhiteSpace(copyCode))
                throw new ArgumentException("copyCode is required", nameof(copyCode));

            var nextId = Store.InventoryRecords.Any() ? Store.InventoryRecords.Max(r => r.Id) + 1 : 1;

            var record = new InventoryRecord
            {
                Id = nextId,
                BookId = bookId,
                CopyCode = copyCode,
                IsAvailable = true,
                CreatedByUserId = createdByUserId,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            Store.InventoryRecords.Add(record);
            return record;
        }

        public bool RemoveCopy(int inventoryRecordId, int performedByUserId = 0)
        {
            var copy = Store.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            if (copy == null) return false;

            //If copy borrowed don't remove
            if (!copy.IsAvailable)
                return false;

            var bookId = copy.BookId;
            Store.InventoryRecords.Remove(copy);

            //If no more copies exist, archive the book
            var anyLeft = Store.InventoryRecords.Any(r => r.BookId == bookId);
            if (!anyLeft)
            {
                BookService.ArchiveBook(bookId, performedByUserId);
            }

            return true;
        }

        public List<InventoryRecord> ListCopiesForBook(int bookId)
        {
            return Store.InventoryRecords
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public List<InventoryRecord> GetAvailableCopies(int bookId)
        {
            return Store.InventoryRecords
                .Where(r => r.BookId == bookId && r.IsAvailable)
                .ToList();
        }

    }
}
