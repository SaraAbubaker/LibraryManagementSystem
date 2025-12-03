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
        public bool ReturnCopy(int inventoryRecordId, int currentUserId)
        {
            Validate.Positive(inventoryRecordId, "inventoryRecordId");
            Validate.Positive(currentUserId, "currentUserId");

            var copy = _inventoryRepo.GetById(inventoryRecordId);

            copy.IsAvailable = true;
            copy.LastModifiedByUserId = currentUserId;
            copy.LastModifiedDate = DateOnly.FromDateTime(DateTime.Today);

            _inventoryRepo.Update(copy);

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
                CreatedDate = DateOnly.FromDateTime(DateTime.Now)
            };

            _inventoryRepo.Add(record);

            return record;
        }

        public bool RemoveCopy(int inventoryRecordId, int performedByUserId)
        {
            Validate.Positive(inventoryRecordId, "inventoryRecordId");
            Validate.Positive(performedByUserId, "performedByUserId");

            var copy = _inventoryRepo.GetById(inventoryRecordId);
            
            copy.IsArchived = true;
            copy.ArchivedByUserId = performedByUserId;
            copy.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _inventoryRepo.Update(copy);

            //Archive the book if no copies remain
            var anyLeft = _inventoryRepo.GetAll()
                .Any(r => r.BookId == copy.BookId && !r.IsArchived);
            if (!anyLeft)
            {
                BookService.ArchiveBook(copy.BookId, performedByUserId);
            }

            return true;
        }

        public List<InventoryRecord> ListCopiesForBook(int bookId)
        {
            return _inventoryRepo.GetAll()
                .Where(r => r.BookId == bookId)
                .OrderBy(r => r.Id)
                .ToList();
        }

        public List<InventoryRecord> GetAvailableCopies(int bookId)
        {
            return _inventoryRepo.GetAll()
                .Where(r => r.BookId == bookId && r.IsAvailable)
                .ToList();
        }

    }
}
