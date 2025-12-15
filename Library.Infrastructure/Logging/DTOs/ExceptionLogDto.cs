
using Library.Infrastructure.Logging.Models;
using System.ComponentModel.DataAnnotations;

namespace Library.Infrastructure.Logging.DTOs
{
    public class ExceptionLogDto
    {
        [Required]
        public Guid Guid { get; set; }

        [Required(ErrorMessage = "Exception message is required.")]
        [StringLength(1000, ErrorMessage = "Exception message cannot exceed 1000 characters.")]
        public required string ExceptionMessage { get; set; }

        [Required(ErrorMessage = "Stack trace is required.")]
        public required string StackTrace { get; set; }

        [Required(ErrorMessage = "Service name is required.")]
        [StringLength(100, ErrorMessage = "Service name cannot exceed 100 characters.")]
        public required string ServiceName { get; set; }

        [Required]
        public LogLevel Level { get; set; } = LogLevel.Exception; //default

        [Required]
        public required DateTime CreatedAt { get; set; }
    }
}
