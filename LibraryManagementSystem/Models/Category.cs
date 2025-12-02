using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class Category : AuditBase
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = null!;

        //many books = 1 category
        public List<Book> Books { get; set; } = new();
    }
}
