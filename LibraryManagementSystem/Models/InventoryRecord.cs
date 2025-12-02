using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class InventoryRecord : AuditBase
    {
        public bool IsAvailable { get; set; } = true;

        [Required]
        [RegularExpression(@"^BC-\d{2}$",
            ErrorMessage = "Copy code must follow format BC-01.")]
        public string CopyCode { get; set; } = null!;

        public int BookId { get; set; } //Foreign Key
        public Book? Book { get; set; }

    }
}
