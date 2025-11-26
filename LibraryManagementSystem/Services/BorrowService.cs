using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly LibraryDataStore Store;
        private readonly InventoryService Inventory;
        private int nextBorrowRecordId = 1;

        public BorrowService(LibraryDataStore store, InventoryService inventoryService)
        {
            Store = store;
            Inventory = inventoryService;

            if (Store.BorrowRecords.Any())
                nextBorrowRecordId = Store.BorrowRecords.Max(r => r.Id) + 1;
        }

        public List<Book> SearchBooks(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Book>();

            title = title.ToLower();

            return Store.Books
                .Where(b => b.Title.ToLower().Contains(title))
                .ToList();
        }

        public bool HasAvailableCopy(int bookId)
        {
            return Inventory.GetAvailableCopies(bookId).Any();
        }

        private InventoryRecord? GetAvailableCopy(int bookId)
        {
            return Inventory
                .GetAvailableCopies(bookId)
                .FirstOrDefault();
        }

        //fix
        public BorrowRecord BorrowBook(RequestBorrowDto dto)
        {
            var borrow = new BorrowRecord
            {
                InventoryRecordId = dto.InventoryRecordId,
                UserId = dto.UserId,
                BorrowDate = DateTime.Now,
                DueDate = dto.DueDate,
                ReturnDate = null
            };

            BorrowRecord.Add(borrow);
            return borrow;
        }


        public bool ReturnBook(int borrowRecordId)
        {
            var record = Store.BorrowRecords.FirstOrDefault(r => r.Id == borrowRecordId);
            if (record == null || record.ReturnDate != null)
                return false;

            record.ReturnDate = DateTime.UtcNow;

            var copy = Store.InventoryRecords.FirstOrDefault(i => i.Id == record.InventoryRecordId);
            if (copy != null)
                copy.IsAvailable = true;

            return true;
        }

        public List<BorrowRecord> GetOverdueRecords()
        {
            var now = DateTime.Now;
            return Store.BorrowRecords
                .Where(r => r.ReturnDate == null && r.DueDate < now)
                .ToList();
        }
    }
}
