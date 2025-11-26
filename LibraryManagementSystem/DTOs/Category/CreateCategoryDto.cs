using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.Category
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
