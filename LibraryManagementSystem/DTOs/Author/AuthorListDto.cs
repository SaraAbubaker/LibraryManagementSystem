using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Author
{
    public class AuthorListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }

        public int BookCount { get; set; }
    }
}
