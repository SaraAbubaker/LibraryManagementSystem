using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.BorrowRecord
{
    public class ReturnBorrowDto
    {
        [Required]
        public int BorrowRecordId { get; set; }
    }
}
