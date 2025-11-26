using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class SearchBookParamsDto
    {
        // Paging
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;

        // Sorting
        public string? SortBy { get; set; } = "Title"; //Title, PublishDate, etc
        public string? SortDir { get; set; } = "asc"; //asc or desc

        // Search
        public string? Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? Version { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public string? Publisher { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
