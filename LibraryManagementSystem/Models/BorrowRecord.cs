using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class BorrowRecord : AuditBase
    {
        [Required]
        public DateOnly BorrowDate { get; set; }
        [Required]
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnDate { get; set; } //nullable

        //Foreign keys
        public int InventoryRecordId { get; set; }
        public int UserId { get; set; }

        //navigation
        public InventoryRecord? InventoryRecord { get; set; }
        public User? User { get; set; }
    }
}
