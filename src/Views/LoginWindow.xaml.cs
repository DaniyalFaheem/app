using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using FaceRecognitionAttendance.ViewModels;
using FaceRecognitionAttendance.Services.Authentication;

namespace FaceRecognitionAttendance.Views
{
    /// <summary>
    /// Login window for authentication
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel? _viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            // Get authentication service from DI container
            var authService = App.ServiceProvider?.GetService<IAuthenticationService>();
            
            if (authService == null)
            {
                MessageBox.Show(
                    "Failed to initialize authentication service.",
                    "Initialization Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
                return;
            }

            _viewModel = new LoginViewModel(authService);
            _viewModel.LoginSuccessful += OnLoginSuccessful;
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        private void OnLoginSuccessful(object? sender, EventArgs e)
        {
            try
            {
                // Open main window
                var mainWindow = new UserManagementWindow();
                mainWindow.Show();

                // Close login window
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to open main window: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.LoginSuccessful -= OnLoginSuccessful;
            }
            base.OnClosed(e);
        }
    }
}
