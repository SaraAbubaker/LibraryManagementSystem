using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Book
{
    public class UpdateBookDto
    {
        [Required]
        public int Id { get; set; } //BookId

        
        [StringLength(200)]
        public string? Title { get; set; }
        
        public DateTime? PublishDate { get; set; }

        [StringLength(50)]
        public string? Version { get; set; }

        [StringLength(200)]
        public string? Publisher { get; set; }

        public int? AuthorId { get; set; }
        
        public int? CategoryId { get; set; }
    }
}
