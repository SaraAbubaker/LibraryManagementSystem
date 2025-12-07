
using System.ComponentModel.DataAnnotations;

namespace Library.Shared.DTOs.BorrowRecord
{
    public class ReturnBorrowDto
    {
        [Required]
        public int BorrowRecordId { get; set; }
    }
}
