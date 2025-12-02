using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LibraryManagementSystem.Models
{
    public class Publisher : AuditBase
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;
    }
}
