using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        
        public string? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
