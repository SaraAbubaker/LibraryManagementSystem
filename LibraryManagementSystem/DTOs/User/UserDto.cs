using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;

        public int BorrowedBooksCount { get; set; }

        
        public int? CreatedByUserId { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
    }
}
