using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.DTOs
{
    public class PublisherDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int InventoryCount { get; set; }

        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? UpdatedByUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsArchived { get; set; }
        public int? ArchivedByUserId { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
