
namespace Library.Shared.DTOs
{
    public class SearchParamsDto
    {
        public string? SearchParam { get; set; } //Title, Author, etc

        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Sorting
        public string? SortBy { get; set; } //Title, PublishDate, etc
        public string? SortDir { get; set; } = "asc"; //asc or desc
    }
}
