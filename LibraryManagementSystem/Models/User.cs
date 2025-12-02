using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class User : AuditBase
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$",
            ErrorMessage = "Username must contain letters, numbers, underscore, dot and dash only.")]
        public string Username { get; set; } = null!;

        //Email validation
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        //Password Validation
        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be at least 8 characters, contain upper, lower case, and a number.")]
        public string Password { get; set; } = null!;


        public List<BorrowRecord> BorrowRecords { get; set; } = new();
    }
}
