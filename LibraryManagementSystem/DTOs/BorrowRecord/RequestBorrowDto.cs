using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.BorrowRecord
{
    public class RequestBorrowDto
    {
        [Required]
        public int InventoryRecordId { get; set; }

        //User picks 
        public DateOnly? DueDate { get; set; }
    }
}
