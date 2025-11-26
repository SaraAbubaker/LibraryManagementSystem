using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.DTOs.InventoryRecord
{
    public class CreateCopyDto
    {
        [Required]
        public int Id {  get; set; }
        [Required]
        [RegularExpression(@"^BC-\d{2}$",
        ErrorMessage = "Copy code must follow format BC-01.")]
        public string CopyCode { get; set; } = null!;

        [Required]
        public int BookId { get; set; }

    }
}
