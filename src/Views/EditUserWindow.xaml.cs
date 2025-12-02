using System.Windows;
using FaceRecognitionAttendance.ViewModels;

namespace FaceRecognitionAttendance.Views
{
    /// <summary>
    /// Interaction logic for EditUserWindow.xaml
    /// </summary>
    public partial class EditUserWindow : Window
    {
        public EditUserWindow()
        {
            InitializeComponent();
        }

        public EditUserWindow(EditUserViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}