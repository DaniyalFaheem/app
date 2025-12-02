using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;

namespace FaceRecognitionAttendance.Services.Salary
{
    /// <summary>
    /// Salary calculator implementation for 3 faculty types
    /// </summary>
    public class SalaryCalculator : ISalaryCalculator
    {
        private readonly IUserRepository _userRepository;
        private readonly IAttendanceRepository _attendanceRepository;

        public SalaryCalculator(IUserRepository userRepository, IAttendanceRepository attendanceRepository)
        {
            _userRepository = userRepository ??  throw new ArgumentNullException(nameof(userRepository));
            _attendanceRepository = attendanceRepository ??  throw new ArgumentNullException(nameof(attendanceRepository));
        }

        public async Task<SalaryCalculationResult?> CalculateSalaryAsync(User faculty, DateTime startDate, DateTime endDate)
        {
            if (faculty. UserType != UserType.Faculty)
                return null;

            if (! faculty.FacultyType.HasValue)
                return null;

            // Calculate total days in range
            var totalDays = (endDate. Date - startDate.Date).Days + 1;

            // Get attendance count for the period
            var presentDays = await _attendanceRepository. GetAttendanceCountAsync(faculty.Id, startDate, endDate);
            var absentDays = totalDays - presentDays;

            decimal baseSalary = 0;
            decimal deductions = 0;
            decimal finalSalary = 0;

            switch (faculty.FacultyType. Value)
            {
                case FacultyType.MonthlyRegular:
                    // Type 1: Monthly salary - deductions for absences
                    baseSalary = faculty.MonthlySalary ??  0;
                    var perDayRate = baseSalary / 30;
                    deductions = perDayRate * absentDays;
                    finalSalary = baseSalary - deductions;
                    break;

                case FacultyType. VisitingFixed:
                    // Type 2: Fixed 30-day contract (no deductions)
                    baseSalary = faculty.FixedSalary ?? 0;
                    deductions = 0;
                    finalSalary = baseSalary;
                    break;

                case FacultyType.VisitingPerDay:
                    // Type 3: Per-day rate × present days
                    baseSalary = faculty.PerDayRate ?? 0;
                    deductions = 0;
                    finalSalary = baseSalary * presentDays;
                    break;
            }

            return new SalaryCalculationResult
            {
                UserId = faculty.Id,
                Name = faculty.Name,
                FacultyType = faculty.FacultyType.Value,
                BaseSalary = baseSalary,
                TotalDays = totalDays,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                Deductions = deductions,
                FinalSalary = finalSalary,
                StartDate = startDate,
                EndDate = endDate,
                Department = faculty.Department
            };
        }

        public async Task<List<SalaryCalculationResult>> CalculateAllSalariesAsync(DateTime startDate, DateTime endDate)
        {
            var results = new List<SalaryCalculationResult>();
            var allFaculty = await _userRepository.GetByTypeAsync(UserType.Faculty);

            foreach (var faculty in allFaculty. Where(f => f.IsActive))
            {
                var result = await CalculateSalaryAsync(faculty, startDate, endDate);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results;
        }

        public async Task<List<SalaryCalculationResult>> CalculateDepartmentSalariesAsync(string department, DateTime startDate, DateTime endDate)
        {
            var results = new List<SalaryCalculationResult>();
            var departmentFaculty = await _userRepository.GetByDepartmentAsync(department);
            var faculty = departmentFaculty.Where(u => u.UserType == UserType.Faculty && u.IsActive);

            foreach (var member in faculty)
            {
                var result = await CalculateSalaryAsync(member, startDate, endDate);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results;
        }
    }
}