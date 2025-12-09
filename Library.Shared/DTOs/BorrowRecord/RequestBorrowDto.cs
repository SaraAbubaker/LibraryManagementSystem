
using Library.Shared.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Library.Shared.DTOs.BorrowRecord
{
    public class RequestBorrowDto
    {
        [Required]
        [Positive]
        public int InventoryRecordId { get; set; }

        //User picks 
        public DateOnly? DueDate { get; set; }
    }
}
