using System.Windows;
using System.Windows.Input;
using FaceRecognitionAttendance.ViewModels;

namespace FaceRecognitionAttendance.Views
{
    /// <summary>
    /// Interaction logic for UserManagementWindow.xaml
    /// </summary>
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
        }

        public UserManagementWindow(UserManagementViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        // Double-click to view details
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is UserManagementViewModel viewModel)
            {
                if (viewModel.ViewDetailsCommand.CanExecute(null))
                {
                    viewModel.ViewDetailsCommand.Execute(null);
                }
            }
        }
    }
}