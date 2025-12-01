using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Entities
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

        [StringLength(200)]
        public string? Publisher { get; set; }


        //Foreign Key Relation
        public int AuthorId { get; set; }  
        public int CategoryId { get; set; }

        //Easy access to Category.Name
        public Category? Category { get; set; }

        //1 book = many records
        public List<InventoryRecord> InventoryRecords { get; set; } = new();
    }
}
