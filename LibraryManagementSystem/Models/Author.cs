using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class Author : AuditBase
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        //Email validation
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(255)]
        public string? Email { get; set; }

        public List<Book> Books { get; set; } = new();

    }
}
