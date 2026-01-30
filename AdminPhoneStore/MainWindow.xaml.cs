using AdminPhoneStore.Views;
using System.Windows;

namespace AdminPhoneStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new DashboardView();
        }
    }
}