using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// Attendance record entity for tracking user attendance
    /// </summary>
    [Table("AttendanceRecords")]
    public class AttendanceRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        [Required]
        public UserType Type { get; set; }

        /// <summary>
        /// Navigation property to user
        /// </summary>
        public virtual User User { get; set; } = null!;
    }
}