using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class SearchBookParamsDto : SearchParamsDto
    {
        public string? Title { get; set; }
        public DateTime? PublishDate { get; set; }
        public string? Version { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
        public string? Publisher { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
