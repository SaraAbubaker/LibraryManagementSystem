using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Models
{
    //Base Parent Class
    public abstract class AuditBase
    {
        public int Id { get; set; }
        public int? CreatedByUserId { get; set; } //nullable
        public DateOnly CreatedDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int? LastModifiedByUserId { get; set; } //nullable
        public DateOnly? LastModifiedDate { get; set; } //nullable
        public bool IsArchived { get; set; } = false;
        public int? ArchivedByUserId { get; set; }
        public DateOnly? ArchivedDate { get; set; }
    }
}
