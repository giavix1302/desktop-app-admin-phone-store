using AdminPhoneStore.ViewModels.Pages;
using System.Windows;

namespace AdminPhoneStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isMaximized = false;
        private double _normalLeft;
        private double _normalTop;
        private double _normalWidth;
        private double _normalHeight;

        public MainWindow(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            
            // Đảm bảo window fullscreen nhưng không che TaskBar
            Loaded += MainWindow_Loaded;
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Maximize window nhưng không che TaskBar
            MaximizeToWorkArea();
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            // Khi WindowState thay đổi, nếu là Maximized thì override bằng WorkArea
            if (WindowState == WindowState.Maximized)
            {
                // Ngăn window che TaskBar bằng cách set lại kích thước
                MaximizeToWorkArea();
            }
        }

        private void MaximizeToWorkArea()
        {
            // Lấy kích thước màn hình làm việc (không bao gồm TaskBar)
            var workingArea = SystemParameters.WorkArea;
            
            // Lưu vị trí và kích thước hiện tại nếu chưa maximize
            if (!_isMaximized)
            {
                _normalLeft = Left;
                _normalTop = Top;
                _normalWidth = Width;
                _normalHeight = Height;
            }
            
            // Đặt vị trí và kích thước window để fullscreen nhưng không che TaskBar
            WindowState = WindowState.Normal; // Phải set về Normal trước
            Left = workingArea.Left;
            Top = workingArea.Top;
            Width = workingArea.Width;
            Height = workingArea.Height;
            _isMaximized = true;
            
            // Cập nhật icon
            MaximizeIcon.Text = "❐"; // Restore icon
        }

        private void RestoreFromMaximized()
        {
            WindowState = WindowState.Normal;
            Left = _normalLeft;
            Top = _normalTop;
            Width = _normalWidth;
            Height = _normalHeight;
            _isMaximized = false;
            
            // Cập nhật icon
            MaximizeIcon.Text = "□"; // Maximize icon
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isMaximized)
            {
                RestoreFromMaximized();
            }
            else
            {
                MaximizeToWorkArea();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // Cho phép kéo window khi click vào header
            if (e.ClickCount == 2)
            {
                // Double click để maximize/restore
                MaximizeButton_Click(sender, e);
            }
            else
            {
                // Drag window
                DragMove();
            }
        }
    }
}