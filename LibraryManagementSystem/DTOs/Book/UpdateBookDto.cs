using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
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

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than zero.")]
        public int UserId { get; set; }


        public int? PublisherId { get; set; }
        public int? AuthorId { get; set; }
        public int? CategoryId { get; set; }
    }
}
