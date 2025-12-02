using LibraryManagementSystem.Data;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class InventoryService
    {
        private readonly LibraryContext _context;
        private readonly BookService BookService;

        public InventoryService(LibraryContext context, BookService bookService)
        {
            _context = context;
            BookService = bookService;
        }

        //Available = true + audit fields set
        public bool ReturnCopy(int inventoryRecordId, int currentUserId)
        {
            Validate.Positive(inventoryRecordId, "inventoryRecordId");
            Validate.Positive(currentUserId, "currentUserId");

            var copy = _context.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            Validate.Exists(copy, $"Inventory record with id {inventoryRecordId}");

            copy!.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateOnly.FromDateTime(DateTime.Today);

            _context.SaveChanges();

            return true;
        }

        //Create, Remove, Read
        public InventoryRecord CreateCopy(int bookId, string copyCode, int createdByUserId)
        {
            Validate.Positive(bookId, "bookId");
            Validate.NotEmpty(copyCode, "copyCode");
            Validate.Positive(createdByUserId, "createdByUserId");
            
            var record = new InventoryRecord
            {
                BookId = bookId,
                CopyCode = copyCode,
                IsAvailable = true,
                CreatedByUserId = createdByUserId,
                CreatedDate = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _context.InventoryRecords.Add(record);
            _context.SaveChanges();

            return record;
        }

        public bool RemoveCopy(int inventoryRecordId, int performedByUserId)
        {
            Validate.Positive(inventoryRecordId, "inventoryRecordId");
            Validate.Positive(performedByUserId, "performedByUserId");

            var copy = _context.InventoryRecords.FirstOrDefault(r => r.Id == inventoryRecordId);
            Validate.Exists(copy, $"Inventory record with id {inventoryRecordId}");

            if (!copy!.IsAvailable)
                throw new ConflictException("Cannot remove a copy that is currently borrowed.");

            var bookId = copy.BookId;
            _context.InventoryRecords.Remove(copy);
            _context.SaveChanges();

            //Archive book if no copies remain
            var anyLeft = _context.InventoryRecords.Any(r => r.BookId == bookId);
            if (!anyLeft)
            {
                BookService.ArchiveBook(bookId, performedByUserId);
            }

            return true;
        }


        public List<InventoryRecord> ListCopiesForBook(int bookId)
        {
            return _context.InventoryRecords
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public List<InventoryRecord> GetAvailableCopies(int bookId)
        {
            return _context.InventoryRecords
                .Where(r => r.BookId == bookId && r.IsAvailable)
                .ToList();
        }

    }
}
