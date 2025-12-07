
namespace Library.Shared.DTOs.BorrowRecord
{
    public class BorrowDto
    {
        public int Id { get; set; }

        public DateOnly BorrowDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public bool IsOverdue { get; set; }
        public int OverdueDays { get; set; }

        public int InventoryRecordId { get; set; }
        public string? CopyCode { get; set; }

        public int UserId { get; set; }
        public string? Username { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateOnly? LastModifiedDate { get; set; }
    }
}
