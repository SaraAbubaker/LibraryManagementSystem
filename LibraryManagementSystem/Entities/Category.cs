using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace LibraryManagementSystem.Entities
{
    public class Category :AuditBase
    {
        [Required]
        public string Name { get; set; } = null!;

        //many books = 1 category
        public List<Book> Books { get; set; } = new();
    }
}
