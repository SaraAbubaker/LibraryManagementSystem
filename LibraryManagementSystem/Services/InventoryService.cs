using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class InventoryService
    {
        //navigation to the lists
        private readonly LibraryDataStore Store;
        public InventoryService(LibraryDataStore store)
        {
            Store = store;
        }
        public List<InventoryRecord> GetAvailableCopies(int bookId)
        {
            return Store.InventoryRecords
                .Where(r => r.BookId == bookId && r.IsAvailable)
                .ToList();
        }

        public bool ReturnCopy(int inventoryRecordId, int currentUserId)
        {
            var copy = Store.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            if (copy == null) return false;

            copy.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateTime.UtcNow;
            return true;
        }


    }
}
