using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// User entity representing both Students and Faculty members
    /// </summary>
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Phone]
        public string Phone { get; set; } = string. Empty;

        [MaxLength(100)]
        [EmailAddress]
        public string?  Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        public UserType UserType { get; set; }

        /// <summary>
        /// Faculty type (only applicable if UserType is Faculty)
        /// </summary>
        public FacultyType?  FacultyType { get; set; }

        /// <summary>
        /// Monthly salary for MonthlyRegular faculty type
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal?  MonthlySalary { get; set; }

        /// <summary>
        /// Fixed salary for VisitingFixed faculty type (30-day contract)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FixedSalary { get; set; }

        /// <summary>
        /// Per-day rate for VisitingPerDay faculty type
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PerDayRate { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 128-dimensional face encoding stored as byte array
        /// </summary>
        [Required]
        public byte[] FaceEncoding { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Path to user's profile photo
        /// </summary>
        [MaxLength(500)]
        public string?  PhotoPath { get; set; }

        /// <summary>
        /// Whether the user is active in the system
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Last modification timestamp for audit trail (NEW for Edit feature)
        /// </summary>
        public DateTime? LastModified { get; set; }

        /// <summary>
        /// Name of admin who last modified this user (NEW for Edit feature)
        /// </summary>
        [MaxLength(100)]
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Navigation property to attendance records
        /// </summary>
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        
        /// <summary>
        /// Clone method for Edit functionality
        /// </summary>
        public User Clone()
        {
            return new User
            {
                Id = this.Id,
                Name = this.Name,
                Phone = this.Phone,
                Email = this.Email,
                Department = this.Department,
                UserType = this.UserType,
                FacultyType = this.FacultyType,
                MonthlySalary = this.MonthlySalary,
                FixedSalary = this. FixedSalary,
                PerDayRate = this.PerDayRate,
                RegistrationDate = this. RegistrationDate,
                FaceEncoding = this.FaceEncoding,
                PhotoPath = this.PhotoPath,
                IsActive = this.IsActive,
                LastModified = this.LastModified,
                ModifiedBy = this.ModifiedBy
            };
        }
    }
}