using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
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
