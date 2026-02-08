using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for CategoryView.xaml
    /// </summary>
    public partial class CategoryView : UserControl
    {
        public CategoryView()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.GetService<CategoryViewModel>();
            DataContext = viewModel;
        }
    }
}
