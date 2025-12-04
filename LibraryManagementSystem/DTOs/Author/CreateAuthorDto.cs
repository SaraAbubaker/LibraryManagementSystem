
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Author
{
    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Author name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2 and 100 characters.")]
        public string Name { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than zero.")]
        public int UserId { get; set; }

        
    }
}
