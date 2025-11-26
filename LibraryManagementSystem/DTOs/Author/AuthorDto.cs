using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Author
{
    public class AuthorDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public int BookCount { get; set; }
    }
}
