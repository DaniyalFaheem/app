using FaceRecognitionAttendance. Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services. Salary
{
    /// <summary>
    /// Salary calculator service interface
    /// </summary>
    public interface ISalaryCalculator
    {
        /// <summary>
        /// Calculate salary for a specific faculty member
        /// </summary>
        Task<SalaryCalculationResult? > CalculateSalaryAsync(User faculty, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate salaries for all faculty members
        /// </summary>
        Task<List<SalaryCalculationResult>> CalculateAllSalariesAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Calculate salaries for specific department
        /// </summary>
        Task<List<SalaryCalculationResult>> CalculateDepartmentSalariesAsync(string department, DateTime startDate, DateTime endDate);
    }
}