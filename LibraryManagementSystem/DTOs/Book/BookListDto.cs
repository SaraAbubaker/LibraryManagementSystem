using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class BookListDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public DateOnly PublishDate { get; set; }
        public string? Version { get; set; }

        public string PublisherName { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public bool IsAvailable { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
        public bool IsArchived { get; set; }
        public int? ArchivedByUserId { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
