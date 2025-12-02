using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;
using FaceRecognitionAttendance.Services.Authentication;
using FaceRecognitionAttendance.Helpers;

namespace FaceRecognitionAttendance.ViewModels
{
    /// <summary>
    /// ViewModel for Edit User Window (NEW FEATURE)
    /// Handles editing of student and faculty information with full validation
    /// </summary>
    public class EditUserViewModel : BaseViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IAuthenticationService _authService;
        private readonly User _originalUser;
        private User _user;
        private bool _dialogResult;

        public EditUserViewModel(
            User user, 
            IUserRepository userRepository, 
            IAdminUserRepository adminUserRepository,
            IAuthenticationService authService)
        {
            _userRepository = userRepository ??  throw new ArgumentNullException(nameof(userRepository));
            _adminUserRepository = adminUserRepository ??  throw new ArgumentNullException(nameof(adminUserRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            
            _originalUser = user ?? throw new ArgumentNullException(nameof(user));
            _user = user. Clone(); // Clone for editing

            // Initialize commands
            SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
            CancelCommand = new RelayCommand(Cancel);

            // Initialize departments and types
            InitializeData();
        }

        #region Properties

        public User User
        {
            get => _user;
            set
            {
                if (SetProperty(ref _user, value))
                {
                    UpdateVisibility();
                }
            }
        }

        public List<string> Departments { get; set; } = new List<string>
        {
            "Computer Science",
            "Electrical Engineering",
            "Mechanical Engineering",
            "Civil Engineering",
            "Business Administration",
            "Mathematics",
            "Physics",
            "Chemistry",
            "Biology"
        };

        public List<UserType> UserTypes { get; set; } = new List<UserType>
        {
            UserType.Student,
            UserType.Faculty
        };

        public List<FacultyType> FacultyTypes { get; set; } = new List<FacultyType>
        {
            FacultyType.MonthlyRegular,
            FacultyType. VisitingFixed,
            FacultyType. VisitingPerDay
        };

        // Dynamic visibility properties
        private bool _isFaculty;
        public bool IsFaculty
        {
            get => _isFaculty;
            set => SetProperty(ref _isFaculty, value);
        }

        private bool _isMonthlyRegular;
        public bool IsMonthlyRegular
        {
            get => _isMonthlyRegular;
            set => SetProperty(ref _isMonthlyRegular, value);
        }

        private bool _isVisitingFixed;
        public bool IsVisitingFixed
        {
            get => _isVisitingFixed;
            set => SetProperty(ref _isVisitingFixed, value);
        }

        private bool _isVisitingPerDay;
        public bool IsVisitingPerDay
        {
            get => _isVisitingPerDay;
            set => SetProperty(ref _isVisitingPerDay, value);
        }

        private string _lastModifiedInfo = string.Empty;
        public string LastModifiedInfo
        {
            get => _lastModifiedInfo;
            set => SetProperty(ref _lastModifiedInfo, value);
        }

        private string _validationError = string.Empty;
        public string ValidationError
        {
            get => _validationError;
            set => SetProperty(ref _validationError, value);
        }

        public bool DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        #region Methods

        private void InitializeData()
        {
            UpdateVisibility();
            UpdateLastModifiedInfo();
            
            // Subscribe to property changes for dynamic validation
            User.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(User.UserType))
                {
                    UpdateVisibility();
                }
                else if (e.PropertyName == nameof(User.FacultyType))
                {
                    UpdateVisibility();
                }
            };
        }

        private void UpdateVisibility()
        {
            IsFaculty = User. UserType == UserType.Faculty;
            
            if (IsFaculty && User.FacultyType. HasValue)
            {
                IsMonthlyRegular = User.FacultyType.Value == FacultyType.MonthlyRegular;
                IsVisitingFixed = User.FacultyType. Value == FacultyType. VisitingFixed;
                IsVisitingPerDay = User. FacultyType.Value == FacultyType.VisitingPerDay;
            }
            else
            {
                IsMonthlyRegular = false;
                IsVisitingFixed = false;
                IsVisitingPerDay = false;
            }
        }

        private void UpdateLastModifiedInfo()
        {
            if (User.LastModified. HasValue && ! string.IsNullOrEmpty(User.ModifiedBy))
            {
                LastModifiedInfo = $"Last modified: {User.LastModified.Value:yyyy-MM-dd HH:mm} by {User.ModifiedBy}";
            }
            else
            {
                LastModifiedInfo = "Never modified";
            }
        }

        private bool CanSave()
        {
            return ValidateInput();
        }

        private bool ValidateInput()
        {
            ValidationError = string.Empty;

            // Basic validation
            if (string.IsNullOrWhiteSpace(User.Name))
            {
                ValidationError = "Name is required. ";
                return false;
            }

            if (string.IsNullOrWhiteSpace(User.Phone))
            {
                ValidationError = "Phone number is required.";
                return false;
            }

            // Phone format validation
            if (User.Phone.Length < 10)
            {
                ValidationError = "Phone number must be at least 10 digits.";
                return false;
            }

            // Email validation (if provided)
            if (!string.IsNullOrWhiteSpace(User.Email))
            {
                if (!IsValidEmail(User.Email))
                {
                    ValidationError = "Email format is invalid.";
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(User.Department))
            {
                ValidationError = "Department is required.";
                return false;
            }

            // Faculty-specific validation
            if (User.UserType == UserType.Faculty)
            {
                if (! User.FacultyType.HasValue)
                {
                    ValidationError = "Faculty type is required for faculty members.";
                    return false;
                }

                switch (User.FacultyType. Value)
                {
                    case FacultyType.MonthlyRegular:
                        if (! User.MonthlySalary.HasValue || User.MonthlySalary.Value <= 0)
                        {
                            ValidationError = "Monthly salary must be greater than 0.";
                            return false;
                        }
                        break;

                    case FacultyType. VisitingFixed:
                        if (!User.FixedSalary.HasValue || User.FixedSalary. Value <= 0)
                        {
                            ValidationError = "Fixed salary must be greater than 0.";
                            return false;
                        }
                        break;

                    case FacultyType.VisitingPerDay:
                        if (!User. PerDayRate.HasValue || User.PerDayRate.Value <= 0)
                        {
                            ValidationError = "Per-day rate must be greater than 0.";
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr. Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async System.Threading.Tasks.Task SaveAsync()
        {
            if (! ValidateInput())
            {
                MessageBox.Show(ValidationError, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Update modification tracking
                User.LastModified = DateTime.Now;
                User.ModifiedBy = _authService.CurrentUser?.Username ?? "Unknown";

                // Save to database
                await _userRepository. UpdateAsync(User);

                // Log audit trail
                await LogAuditAsync();

                MessageBox.Show("User information updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                DialogResult = true;
                CloseWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task LogAuditAsync()
        {
            var changes = GetChanges();
            
            if (! string.IsNullOrEmpty(changes))
            {
                var auditLog = new AuditLog
                {
                    UserId = _authService.CurrentUser?.Id ?? 0,
                    Action = $"Updated user: {User.Name} (ID: {User.Id})",
                    Details = changes,
                    Timestamp = DateTime.Now
                };

                await _adminUserRepository.AddAuditLogAsync(auditLog);
            }
        }

        private string GetChanges()
        {
            var changes = new List<string>();

            if (_originalUser.Name != User.Name)
                changes.Add($"Name: '{_originalUser.Name}' → '{User.Name}'");

            if (_originalUser.Phone != User.Phone)
                changes.Add($"Phone: '{_originalUser.Phone}' → '{User. Phone}'");

            if (_originalUser.Email != User.Email)
                changes.Add($"Email: '{_originalUser.Email ??  "None"}' → '{User.Email ?? "None"}'");

            if (_originalUser.Department != User.Department)
                changes.Add($"Department: '{_originalUser.Department}' → '{User.Department}'");

            if (_originalUser.UserType != User.UserType)
                changes.Add($"User Type: '{_originalUser.UserType}' → '{User.UserType}'");

            if (_originalUser. FacultyType != User.FacultyType)
                changes. Add($"Faculty Type: '{_originalUser.FacultyType}' → '{User.FacultyType}'");

            if (_originalUser.MonthlySalary != User.MonthlySalary)
                changes.Add($"Monthly Salary: '{_originalUser.MonthlySalary:C}' → '{User.MonthlySalary:C}'");

            if (_originalUser. FixedSalary != User.FixedSalary)
                changes.Add($"Fixed Salary: '{_originalUser. FixedSalary:C}' → '{User.FixedSalary:C}'");

            if (_originalUser. PerDayRate != User. PerDayRate)
                changes.Add($"Per-Day Rate: '{_originalUser. PerDayRate:C}' → '{User.PerDayRate:C}'");

            if (_originalUser.IsActive != User.IsActive)
                changes.Add($"Status: '{(_originalUser.IsActive ? "Active" : "Inactive")}' → '{(User.IsActive ? "Active" : "Inactive")}'");

            return string.Join("; ", changes);
        }

        private void Cancel()
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel?  Any unsaved changes will be lost.",
                "Confirm Cancel",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            // Close the window - implementation depends on Window reference
            Application.Current.Windows. OfType<Window>()
                .FirstOrDefault(w => w.DataContext == this)?.Close();
        }

        #endregion
    }
}