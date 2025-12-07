
namespace Library.Shared.DTOs.Book
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;
        public DateOnly PublishDate { get; set; }
        public string? Version { get; set; }

        public int PublisherId { get; set; }
        public string PublisherName { get; set; } = null!;

        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = null!;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;

        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
        public bool IsArchived { get; set; }
        public int? ArchivedByUserId { get; set; }
        public DateOnly? ArchivedDate { get; set; }
    }
}
