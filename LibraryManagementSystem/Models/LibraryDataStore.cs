using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Entities
{
    public class LibraryDataStore
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public List<Author> Authors { get; set; } = new List<Author>
        {
            new Author { Id = 0, Name = "Unknown", Email = string.Empty }
        };
        public List<Category> Categories { get; set; } = new List<Category>
        {
            new Category { Id = 0, Name = "Unknown" }
        };
        public List<InventoryRecord> InventoryRecords { get; set; } = new List<InventoryRecord>();
        public List<User> Users { get; set; } = new List<User>();
        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();
    }
}
