using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.BorrowRecord
{
    public class BorrowDto
    {
        public int Id { get; set; }

        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int InventoryRecordId { get; set; }
        public string? CopyCode { get; set; }

        public int UserId { get; set; }
        public string? Username { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
