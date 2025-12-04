using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.UserType
{
    public class UserTypeDto
    {
        public int Id { get; set; }
        public string Role { get; set; } = null!;
        public int UserId { get; set; }
    }
}
