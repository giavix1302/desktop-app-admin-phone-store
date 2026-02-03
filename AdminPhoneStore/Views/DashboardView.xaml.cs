using System.Windows.Controls;
using AdminPhoneStore.ViewModels;
using AdminPhoneStore.Infrastructure;

namespace AdminPhoneStore.Views
{
    /// <summary>
    /// Interaction logic for DashboardView.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            // Resolve ViewModel từ DI container
            DataContext = ServiceLocator.GetService<DashboardViewModel>();
        }
    }
}
