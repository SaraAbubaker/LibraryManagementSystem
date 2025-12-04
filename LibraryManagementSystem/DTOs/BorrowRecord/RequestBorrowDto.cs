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

        [Required(ErrorMessage = "User ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be greater than zero.")]
        public int UserId { get; set; }

        //User picks 
        public DateOnly? DueDate { get; set; }
    }
}
