using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Entities
{
    public class BorrowRecord : AuditBase
    {
        [Required]
        public DateTime BorrowDate { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } //nullable

        //Foreign keys
        public int InventoryRecordId { get; set; }
        public int UserId { get; set; }

        //navigation
        public InventoryRecord? InventoryRecord { get; set; }
        public User? User { get; set; }
    }
}
