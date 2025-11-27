using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Entities
{
    //Base Parent Class
    public abstract class AuditBase
    {
        public int Id { get; set; }
        public int? CreatedByUserId { get; set; } //nullable
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? LastModifiedByUserId { get; set; } //nullable
        public DateTime? LastModifiedDate { get; set; } //nullable
        public bool IsArchived { get; set; } = false;
        public int? ArchivedByUserId { get; set; }
        public DateTime? ArchivedDate { get; set; }
    }
}
