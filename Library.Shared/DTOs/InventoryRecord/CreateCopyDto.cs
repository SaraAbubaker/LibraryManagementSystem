
using System.ComponentModel.DataAnnotations;

namespace Library.Shared.DTOs.InventoryRecord
{
    public class CreateCopyDto
    {
        [Required]
        [RegularExpression(@"^BC-\d{2}$",
        ErrorMessage = "Copy code must follow format BC-01.")]
        public string CopyCode { get; set; } = null!;

        [Required]
        public int BookId { get; set; }

    }
}
