using System.Windows;
using AdminPhoneStore.ViewModels.Auth;

namespace AdminPhoneStore.Windows.Auth
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Đảm bảo window fullscreen nhưng không che TaskBar
            Loaded += LoginWindow_Loaded;
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Lấy kích thước màn hình làm việc (không bao gồm TaskBar)
            var workingArea = SystemParameters.WorkArea;
            
            // Đặt vị trí và kích thước window để fullscreen nhưng không che TaskBar
            Left = workingArea.Left;
            Top = workingArea.Top;
            Width = workingArea.Width;
            Height = workingArea.Height;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
