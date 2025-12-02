using FaceRecognitionAttendance.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FaceRecognitionAttendance.Services.Attendance
{
    /// <summary>
    /// CSV export service interface
    /// </summary>
    public interface ICsvExportService
    {
        /// <summary>
        /// Export single attendance record to CSV
        /// </summary>
        Task ExportAttendanceAsync(AttendanceRecord record);

        /// <summary>
        /// Export multiple attendance records to CSV
        /// </summary>
        Task ExportAttendanceListAsync(List<AttendanceRecord> records, string fileName);

        /// <summary>
        /// Export salary calculation results to CSV
        /// </summary>
        Task ExportSalaryReportAsync(List<SalaryCalculationResult> results, DateTime startDate, DateTime endDate);
    }
}