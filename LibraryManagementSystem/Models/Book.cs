using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class Book : AuditBase
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        public DateOnly PublishDate { get; set; }

        [StringLength(50)]
        public string? Version { get; set; }


        //Foreign Key Relation
        public int AuthorId { get; set; }  
        public int CategoryId { get; set; }
        public int PublisherId { get; set; }

        //Easy access to Category.Name
        public Author? Author { get; set; }
        public Category? Category { get; set; }
        public Publisher? Publisher { get; set; }

        //1 book = many records
        public List<InventoryRecord> InventoryRecords { get; set; } = new();
    }
}
