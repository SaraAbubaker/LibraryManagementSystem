using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class CreateBookDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;
        [Required]
        public DateOnly PublishDate { get; set; }
        [StringLength(50)]
        public string? Version { get; set; }
        [StringLength(200)]
        public string? Publisher { get; set; }

        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
