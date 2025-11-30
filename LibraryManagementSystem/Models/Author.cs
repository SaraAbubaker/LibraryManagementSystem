using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Entities
{
    public class Author : AuditBase
    {
        [Required]
        public string Name { get; set; } = null!;

        //Email validation
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }


        public List<Book> Books { get; set; } = new();

    }
}
