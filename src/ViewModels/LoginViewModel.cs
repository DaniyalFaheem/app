using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FaceRecognitionAttendance.Helpers;
using FaceRecognitionAttendance.Services.Authentication;

namespace FaceRecognitionAttendance.ViewModels
{
    /// <summary>
    /// ViewModel for Login Window
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ErrorMessage = string.Empty;
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ErrorMessage = string.Empty;
                    ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        public event EventHandler? LoginSuccessful;

        private bool CanLogin()
        {
            return !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !IsLoading;
        }

        private async Task LoginAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                bool success = await _authService.LoginAsync(Username, Password);

                if (success)
                {
                    LoginSuccessful?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ErrorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }
    }
}
