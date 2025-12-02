using System;

namespace FaceRecognitionAttendance.Models
{
    /// <summary>
    /// Result model for salary calculation operations
    /// </summary>
    public class SalaryCalculationResult
    {
        public int UserId { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public FacultyType FacultyType { get; set; }
        
        public decimal BaseSalary { get; set; }
        
        public int TotalDays { get; set; }
        
        public int PresentDays { get; set; }
        
        public int AbsentDays { get; set; }
        
        public decimal Deductions { get; set; }
        
        public decimal FinalSalary { get; set; }
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
        
        public string Department { get; set; } = string.Empty;
    }
}