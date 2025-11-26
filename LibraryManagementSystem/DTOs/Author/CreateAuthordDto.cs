
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Author
{
    public class CreateAuthordDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }
    }
}
