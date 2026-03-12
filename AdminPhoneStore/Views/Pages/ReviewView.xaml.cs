using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    public partial class ReviewView : UserControl
    {
        public ReviewView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.GetService<ReviewViewModel>();
        }
    }
}
