using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// Audit log entity for tracking sensitive operations (includes Edit tracking)
    /// </summary>
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("AdminUser")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string?  Details { get; set; }

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property to admin user
        /// </summary>
        public virtual AdminUser AdminUser { get; set; } = null!;
    }
}