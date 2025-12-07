
namespace Library.Shared.DTOs.Author
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
        public bool IsArchived { get; set; }
        public int? ArchivedByUserId { get; set; }
        public DateOnly? ArchivedDate { get; set; }

        public int BookCount { get; set; }
    }
}
