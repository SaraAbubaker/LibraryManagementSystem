using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.UserType
{
    public class UpdateUserTypeDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Role { get; set; } = null!;
    }
}
