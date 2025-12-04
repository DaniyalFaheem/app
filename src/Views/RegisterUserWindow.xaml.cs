using System;
using System.Windows;
using FaceRecognitionAttendance.ViewModels;

namespace FaceRecognitionAttendance.Views
{
    /// <summary>
    /// Interaction logic for RegisterUserWindow.xaml
    /// </summary>
    public partial class RegisterUserWindow : Window
    {
        private readonly RegisterUserViewModel? _viewModel;

        public RegisterUserWindow()
        {
            InitializeComponent();
        }

        public RegisterUserWindow(RegisterUserViewModel viewModel) : this()
        {
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;

            // Subscribe to DialogResult changes
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RegisterUserViewModel.DialogResult))
            {
                DialogResult = _viewModel?.DialogResult;
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Cleanup camera and resources
            _viewModel?.Cleanup();
            
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            base.OnClosing(e);
        }
    }
}
