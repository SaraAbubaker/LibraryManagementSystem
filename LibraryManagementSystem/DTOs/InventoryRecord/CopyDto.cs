using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.DTOs.InventoryRecord
{
    public class CopyDto
    {
        public int Id { get; set; }

        public string CopyCode { get; set; } = null!;
        public bool IsAvailable { get; set; }

        public int BookId { get; set; }
        public string? BookTitle { get; set; }

        
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
