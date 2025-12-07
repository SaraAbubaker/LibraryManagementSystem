
using System.ComponentModel.DataAnnotations;

namespace Library.Shared.DTOs.BorrowRecord
{
    public class RequestBorrowDto
    {
        [Required]
        public int InventoryRecordId { get; set; }

        //User picks 
        public DateOnly? DueDate { get; set; }
    }
}
