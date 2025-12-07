
namespace Library.Shared.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        
        public int? CreatedByUserId { get; set; }
        public DateOnly? CreatedDate { get; set; }

        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
    }
}
