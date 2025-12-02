using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.BorrowRecord;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class BorrowService
    {
        private readonly LibraryContext _context;
        private readonly InventoryService Inventory;
        private int nextBorrowRecordId = 1;

        public BorrowService(LibraryContext context, InventoryService inventoryService)
        {
            _context = context;
            Inventory = inventoryService;

            nextBorrowRecordId = _context.BorrowRecords.Any()
                ? _context.BorrowRecords.Max(r => r.Id) + 1
                : 1; //start at 1 if empty
        }

        //ListAll
        public List<BorrowDto> GetBorrowDetails()
        {
            var query =
                from b in _context.BorrowRecords
                join i in _context.InventoryRecords on b.InventoryRecordId equals i.Id into invGroup
                from inv in invGroup.DefaultIfEmpty()
                join u in _context.Users on b.UserId equals u.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                select new BorrowDto
                {
                    Id = b.Id,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    CopyCode = inv.CopyCode,
                    Username = user.Username,
                    IsOverdue = IsBorrowOverdue(b),
                    OverdueDays = CalculateOverdueDays(b)
                };

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

            var borrow = new BorrowRecord
            {
                Id = nextBorrowRecordId++,
                InventoryRecordId = dto.InventoryRecordId,
                UserId = userId,
                BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                DueDate = dto.DueDate ?? DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                ReturnDate = null
            };

            _context.BorrowRecords.Add(borrow);

            if (_context.InventoryRecords.FirstOrDefault(i => i.Id == dto.InventoryRecordId) is not InventoryRecord copy)
                throw new NotFoundException($"Inventory record with id {dto.InventoryRecordId} not found.");

            copy.IsAvailable = false;

            return borrow;
        }

        public bool ReturnBook(int borrowRecordId, int currentUserId)
        {
            Validate.Positive(borrowRecordId, nameof(borrowRecordId));
            Validate.Positive(currentUserId, nameof(currentUserId));

            if (_context.BorrowRecords.FirstOrDefault(r => r.Id == borrowRecordId) is not BorrowRecord record)
                throw new NotFoundException($"Borrow record with id {borrowRecordId} not found.");
            if (record.ReturnDate != null)
                throw new ConflictException($"Borrow record with id {borrowRecordId} has already been returned.");

            //Borrow Record update
            record.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            record.LastModifiedByUserId = currentUserId;
            record.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            //Inventory Record update
            var success = Inventory.ReturnCopy(record.InventoryRecordId, currentUserId);

            //true if both updates worked
            return success;
        }

        //Overdue Logic
        public List<BorrowRecord> GetOverdueRecords()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            return _context.BorrowRecords
                .Where(r => r.ReturnDate == null &&
                            r.DueDate < today)
                .ToList();
        }

        public bool IsBorrowOverdue(BorrowRecord record)
        {
            if (record.ReturnDate != null)
                return false;

            var today = DateOnly.FromDateTime(DateTime.Now);
            return today > record.DueDate;
        }

        public int CalculateOverdueDays(BorrowRecord record)
        {
            var endDate = record.ReturnDate ?? DateOnly.FromDateTime(DateTime.Today);
            var dueDate = record.DueDate;

            //if not overdue, return 0
            if (endDate <= dueDate) return 0;

            var days = (endDate.ToDateTime(TimeOnly.MinValue) - dueDate.ToDateTime(TimeOnly.MinValue)).Days;
            return Math.Max(0, days);
        }

    }
}
