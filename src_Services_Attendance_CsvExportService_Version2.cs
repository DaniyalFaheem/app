using System;
using System.Collections.Generic;
using System. Globalization;
using System.IO;
using System. Linq;
using System.Threading. Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FaceRecognitionAttendance. Models;

namespace FaceRecognitionAttendance.Services.Attendance
{
    /// <summary>
    /// CSV export service implementation
    /// </summary>
    public class CsvExportService : ICsvExportService
    {
        private readonly string _exportPath;

        public CsvExportService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _exportPath = Path.Combine(appDataPath, "FaceRecognitionAttendance", "Exports");
            
            // Create directory if it doesn't exist
            Directory.CreateDirectory(_exportPath);
        }

        public async Task ExportAttendanceAsync(AttendanceRecord record)
        {
            try
            {
                // Determine file name based on user type
                var fileName = record.Type == UserType.Student
                    ? $"Students_Attendance_{DateTime.Now:yyyy-MM-dd}.csv"
                    : $"Faculty_Attendance_{DateTime.Now:yyyy-MM-dd}.csv";

                var filePath = Path.Combine(_exportPath, fileName);

                // Check if file exists
                var fileExists = File.Exists(filePath);

                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = ! fileExists
                };

                using var stream = File.Open(filePath, FileMode. Append, FileAccess.Write, FileShare.Read);
                using var writer = new StreamWriter(stream);
                using var csv = new CsvWriter(writer, config);

                // Write header if new file
                if (!fileExists)
                {
                    csv. WriteField("Name");
                    csv.WriteField("ID");
                    csv.WriteField("Department");
                    csv. WriteField("Date");
                    csv.WriteField("Time");
                    await csv.NextRecordAsync();
                }

                // Write record
                csv.WriteField(record.Name);
                csv.WriteField(record.UserId);
                csv.WriteField(record.Department);
                csv.WriteField(record.DateTime. ToString("yyyy-MM-dd"));
                csv.WriteField(record.DateTime.ToString("HH:mm:ss"));
                await csv.NextRecordAsync();

                Console.WriteLine($"Attendance exported to {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting attendance: {ex.Message}");
            }
        }

        public async Task ExportAttendanceListAsync(List<AttendanceRecord> records, string fileName)
        {
            try
            {
                var filePath = Path.Combine(_exportPath, fileName);

                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Write header
                csv.WriteField("Name");
                csv.WriteField("ID");
                csv. WriteField("Department");
                csv.WriteField("Type");
                csv.WriteField("Date");
                csv.WriteField("Time");
                await csv. NextRecordAsync();

                // Write records
                foreach (var record in records)
                {
                    csv.WriteField(record. Name);
                    csv.WriteField(record.UserId);
                    csv.WriteField(record.Department);
                    csv. WriteField(record.Type.ToString());
                    csv.WriteField(record.DateTime.ToString("yyyy-MM-dd"));
                    csv.WriteField(record.DateTime.ToString("HH:mm:ss"));
                    await csv. NextRecordAsync();
                }

                Console.WriteLine($"Attendance list exported to {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting attendance list: {ex.Message}");
            }
        }

        public async Task ExportSalaryReportAsync(List<SalaryCalculationResult> results, DateTime startDate, DateTime endDate)
        {
            try
            {
                var fileName = $"Salary_Report_{startDate:yyyy-MM-dd}_to_{endDate:yyyy-MM-dd}.csv";
                var filePath = Path.Combine(_exportPath, fileName);

                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                // Write header
                csv.WriteField("Name");
                csv.WriteField("Department");
                csv.WriteField("Faculty Type");
                csv.WriteField("Base Salary");
                csv. WriteField("Total Days");
                csv.WriteField("Present Days");
                csv.WriteField("Absent Days");
                csv.WriteField("Deductions");
                csv.WriteField("Final Salary");
                csv.WriteField("Period Start");
                csv.WriteField("Period End");
                await csv.NextRecordAsync();

                // Write records
                foreach (var result in results)
                {
                    csv.WriteField(result.Name);
                    csv.WriteField(result.Department);
                    csv.WriteField(result.FacultyType. ToString());
                    csv.WriteField(result.BaseSalary.ToString("N2"));
                    csv.WriteField(result.TotalDays);
                    csv.WriteField(result.PresentDays);
                    csv.WriteField(result.AbsentDays);
                    csv.WriteField(result. Deductions.ToString("N2"));
                    csv.WriteField(result.FinalSalary.ToString("N2"));
                    csv.WriteField(result. StartDate.ToString("yyyy-MM-dd"));
                    csv. WriteField(result.EndDate. ToString("yyyy-MM-dd"));
                    await csv.NextRecordAsync();
                }

                Console.WriteLine($"Salary report exported to {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting salary report: {ex.Message}");
            }
        }
    }
}