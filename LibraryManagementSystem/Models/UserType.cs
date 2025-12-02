using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class UserType : AuditBase
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Role { get; set; } = null!; //"Admin", "Normal"
        public List<User> Users { get; set; } = new();
    }
}
