
using System.ComponentModel.DataAnnotations;

namespace Library.Shared.DTOs.Book
{
    public class UpdateBookDto
    {
        [Required]
        public int Id { get; set; }
        
        [StringLength(200)]
        public string? Title { get; set; }
        public DateOnly? PublishDate { get; set; }
        [StringLength(50)]
        public string? Version { get; set; }

        public int? PublisherId { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
    }
}
