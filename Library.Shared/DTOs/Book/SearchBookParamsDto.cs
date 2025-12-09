
using Library.Shared.Helpers;

namespace Library.Shared.DTOs.Book
{
    public class SearchBookParamsDto : SearchParamsDto
    {
        public string? Title { get; set; }
        public DateOnly? PublishDate { get; set; }
        public string? Version { get; set; }

        [Positive]
        public int? AuthorId { get; set; }
        [Positive]
        public int? CategoryId { get; set; }
        [Positive]
        public int? PublisherId { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
