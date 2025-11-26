using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class BookListDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public DateTime PublishDate { get; set; }
        public string? Version { get; set; }
        public string? Publisher { get; set; }

        public string AuthorName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public bool IsAvailable { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
