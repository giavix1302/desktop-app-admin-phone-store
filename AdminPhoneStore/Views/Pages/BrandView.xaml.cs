using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for BrandView.xaml
    /// </summary>
    public partial class BrandView : UserControl
    {
        public BrandView()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.GetService<BrandViewModel>();
            DataContext = viewModel;
        }
    }
}
