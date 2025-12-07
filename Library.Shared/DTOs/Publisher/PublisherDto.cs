
namespace Library.Shared.DTOs.Publisher
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int InventoryCount { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateOnly CreatedDate { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public bool IsArchived { get; set; }
        public int? ArchivedByUserId { get; set; }
        public DateOnly? ArchivedDate { get; set; }
    }
}
