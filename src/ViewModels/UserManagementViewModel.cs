using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FaceRecognitionAttendance.Models;
using FaceRecognitionAttendance.Data.Repositories;
using FaceRecognitionAttendance.Services.Authentication;
using FaceRecognitionAttendance.Views;
using FaceRecognitionAttendance.Helpers;

namespace FaceRecognitionAttendance.ViewModels
{
    /// <summary>
    /// ViewModel for User Management Window
    /// Handles viewing, searching, editing, and deleting users
    /// </summary>
    public class UserManagementViewModel : BaseViewModel
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminUserRepository _adminUserRepository;
        private readonly IAuthenticationService _authService;

        public UserManagementViewModel(
            IUserRepository userRepository,
            IAdminUserRepository adminUserRepository,
            IAuthenticationService authService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _adminUserRepository = adminUserRepository ?? throw new ArgumentNullException(nameof(adminUserRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            Users = new ObservableCollection<User>();
            
            // Initialize commands
            EditUserCommand = new RelayCommand(EditUser, CanEditUser);
            DeleteUserCommand = new RelayCommand(DeleteUser, CanDeleteUser);
            ViewDetailsCommand = new RelayCommand(ViewDetails, CanViewDetails);
            SearchCommand = new RelayCommand(async () => await SearchAsync());
            RefreshCommand = new RelayCommand(async () => await LoadUsersAsync());

            // Load users on initialization
            _ = LoadUsersAsync();
        }

        #region Properties

        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User? _selectedUser;
        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    ((RelayCommand)EditUserCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)DeleteUserCommand).RaiseCanExecuteChanged();
                    ((RelayCommand)ViewDetailsCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private string _filterUserType = "All Users";
        public string FilterUserType
        {
            get => _filterUserType;
            set
            {
                if (SetProperty(ref _filterUserType, value))
                {
                    _ = LoadUsersAsync();
                }
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private int _totalUsers;
        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        #endregion

        #region Commands

        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand ViewDetailsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        #endregion

        #region Methods

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            IsLoading = true;

            try
            {
                var users = FilterUserType switch
                {
                    "Students Only" => await _userRepository.GetByTypeAsync(UserType.Student),
                    "Faculty Only" => await _userRepository.GetByTypeAsync(UserType.Faculty),
                    _ => await _userRepository.GetAllAsync()
                };

                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                TotalUsers = Users.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async System.Threading.Tasks.Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                await LoadUsersAsync();
                return;
            }

            IsLoading = true;

            try
            {
                var users = await _userRepository.SearchAsync(SearchText);

                // Apply filter
                if (FilterUserType == "Students Only")
                    users = users.Where(u => u.UserType == UserType.Student).ToList();
                else if (FilterUserType == "Faculty Only")
                    users = users.Where(u => u.UserType == UserType.Faculty).ToList();

                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                TotalUsers = Users.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanEditUser()
        {
            return SelectedUser != null;
        }

        private void EditUser()
        {
            if (SelectedUser == null) return;

            try
            {
                // Create EditUserViewModel
                var editViewModel = new EditUserViewModel(
                    SelectedUser, 
                    _userRepository, 
                    _adminUserRepository, 
                    _authService);

                // Open EditUserWindow
                var editWindow = new EditUserWindow(editViewModel);
                var result = editWindow.ShowDialog();

                // Refresh list if changes were saved
                if (result == true)
                {
                    _ = LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening edit window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanDeleteUser()
        {
            return SelectedUser != null;
        }

        private async void DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete user '{SelectedUser.Name}'?\n\n" +
                $"This will permanently delete:\n" +
                $"• User profile\n" +
                $"• All attendance records\n" +
                $"• Face recognition data\n\n" +
                $"This action cannot be undone! ",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var userId = SelectedUser.Id;
                var userName = SelectedUser.Name;

                // Delete from database
                var deleted = await _userRepository.DeleteAsync(userId);

                if (deleted)
                {
                    // Log audit
                    await LogAuditAsync($"Deleted user: {userName} (ID: {userId})");

                    // Remove from collection
                    Users.Remove(SelectedUser);
                    TotalUsers = Users.Count;

                    MessageBox.Show($"User '{userName}' deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanViewDetails()
        {
            return SelectedUser != null;
        }

        private void ViewDetails()
        {
            if (SelectedUser == null) return;

            var details = $"User Details\n\n" +
                         $"ID: {SelectedUser.Id}\n" +
                         $"Name: {SelectedUser.Name}\n" +
                         $"Phone: {SelectedUser. Phone}\n" +
                         $"Email: {SelectedUser.Email ?? "N/A"}\n" +
                         $"Department: {SelectedUser.Department}\n" +
                         $"Type: {SelectedUser.UserType}\n";

            if (SelectedUser.UserType == UserType.Faculty && SelectedUser.FacultyType.HasValue)
            {
                details += $"Faculty Type: {SelectedUser.FacultyType.Value}\n";
                
                switch (SelectedUser.FacultyType.Value)
                {
                    case FacultyType.MonthlyRegular:
                        details += $"Monthly Salary: {SelectedUser.MonthlySalary:C}\n";
                        break;
                    case FacultyType.VisitingFixed:
                        details += $"Fixed Salary: {SelectedUser. FixedSalary:C}\n";
                        break;
                    case FacultyType.VisitingPerDay:
                        details += $"Per-Day Rate: {SelectedUser.PerDayRate:C}\n";
                        break;
                }
            }

            details += $"Registered: {SelectedUser. RegistrationDate:yyyy-MM-dd}\n" +
                      $"Status: {(SelectedUser.IsActive ? "Active" : "Inactive")}\n";

            if (SelectedUser.LastModified. HasValue)
            {
                details += $"Last Modified: {SelectedUser.LastModified. Value:yyyy-MM-dd HH:mm} by {SelectedUser.ModifiedBy}";
            }

            MessageBox.Show(details, "User Details", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async System.Threading.Tasks.Task LogAuditAsync(string action, string? details = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    UserId = _authService.CurrentUser?.Id ?? 0,
                    Action = action,
                    Details = details,
                    Timestamp = DateTime.Now
                };

                await _adminUserRepository. AddAuditLogAsync(auditLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log audit: {ex.Message}");
            }
        }

        #endregion
    }
}