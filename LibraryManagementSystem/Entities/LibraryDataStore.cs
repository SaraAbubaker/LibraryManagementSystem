using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Entities
{
    public class LibraryDataStore
    {
        public List<Book> Books { get; set; } = [];
        public List<Author> Authors { get; set; } = new List<Author>
        {
            new Author { Id = 0, Name = "Unknown", Email = string.Empty }
        };
        public List<Category> Categories { get; set; } = [];
        public List<InventoryRecord> InventoryRecords { get; set; } = [];
        public List<User> Users { get; set; } = [];
        public List<BorrowRecord> BorrowRecords { get; set; } = [];

    }
}
