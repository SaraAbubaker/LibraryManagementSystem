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

        public InventoryService(LibraryDataStore store)
        {
            Store = store;
        }

        //Available = true + audit fields set
        public bool ReturnCopy(int inventoryRecordId, int currentUserId)
        {
            var copy = Store.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            if (copy == null) return false;

            copy.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateTime.Now;
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
                CreatedDate = DateTime.UtcNow
            };

            Store.InventoryRecords.Add(record);
            return record;
        }

        public bool RemoveCopy(int inventoryRecordId)
        {
            var copy = Store.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            if (copy == null) return false;

            //Don't remove if copy is borrowed
            if (!copy.IsAvailable)
                return false;

            Store.InventoryRecords.Remove(copy);
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
