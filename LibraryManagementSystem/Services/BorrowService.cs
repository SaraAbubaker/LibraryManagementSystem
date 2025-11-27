using LibraryManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Mapster;
using LibraryManagementSystem.DTOs.BorrowRecord;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly LibraryDataStore Store;
        private readonly InventoryService Inventory;
        private readonly List<BorrowRecord> BorrowRecords;
        private int nextBorrowRecordId = 1;

        //Constructor / Dependancy Injection
        public BorrowService(LibraryDataStore store, InventoryService inventoryService)
        {
            Store = store;
            Inventory = inventoryService;
            BorrowRecords = Store.BorrowRecords;

            if (Store.BorrowRecords.Any())
                nextBorrowRecordId = Store.BorrowRecords.Max(r => r.Id) + 1;
        }

        //ListOne
        public List<Book> SearchBooks(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<Book>();

            title = title.ToLower();

            return Store.Books
                .Where(b => b.Title.ToLower().Contains(title))
                .ToList();
        }

        //ListAll
        public List<BorrowDto> GetBorrowDetails()
        {
            return BorrowRecords
                .Select(b =>
                {
                    var dto = b.Adapt<BorrowDto>();

                    var inventory = Store.InventoryRecords
                        .FirstOrDefault(i => i.Id == b.InventoryRecordId);

                    var user = Store.Users
                        .FirstOrDefault(u => u.Id == b.UserId);

                    dto.CopyCode = inventory?.CopyCode;
                    dto.Username = user?.Username;

                    dto.IsOverdue = IsBorrowOverdue(b);
                    dto.OverdueDays = CalculateOverdueDays(b);

                    return dto;
                })
                .ToList();
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
        public BorrowRecord BorrowBook(RequestBorrowDto dto)
        {
            var borrow = new BorrowRecord
            {
                Id = nextBorrowRecordId++,
                InventoryRecordId = dto.InventoryRecordId,
                UserId = dto.UserId,
                BorrowDate = DateTime.Now,
                DueDate = dto.DueDate ?? DateTime.Now.AddDays(14),
                ReturnDate = null
            };

            BorrowRecords.Add(borrow);

            var copy = Store.InventoryRecords.FirstOrDefault(i => i.Id == dto.InventoryRecordId);
            if (copy != null)
                copy.IsAvailable = false;

            return borrow;
        }

        public bool ReturnBook(int borrowRecordId, int currentUserId)
        {
            var record = Store.BorrowRecords.FirstOrDefault(r => r.Id == borrowRecordId);
            if (record == null || record.ReturnDate != null)
                return false;

            //Borrow Record update
            record.ReturnDate = DateTime.Now;
            record.LastModifiedByUserId = currentUserId;
            record.LastModifiedDate = DateTime.Now;

            //Inventory Record update
            var success = Inventory.ReturnCopy(record.InventoryRecordId, currentUserId);

            //true if both updates worked
            return success;
        }

        //Overdue Logic
        public List<BorrowRecord> GetOverdueRecords()
        {
            var now = DateTime.Now;
            return Store.BorrowRecords
                .Where(r => r.ReturnDate == null && r.DueDate < now)
                .ToList();
        }

        public bool IsBorrowOverdue(BorrowRecord record)
        {
            if (record.ReturnDate != null)
                return false;

            return DateTime.Now > record.DueDate;
        }

        public int CalculateOverdueDays(BorrowRecord record)
        {
            // If returned show return date otherwise show today
            var endDate = record.ReturnDate ?? DateTime.Now;

            //not overdue = 0
            if (endDate <= record.DueDate)
                return 0;

            return (endDate - record.DueDate).Days;
        }

    }
}
