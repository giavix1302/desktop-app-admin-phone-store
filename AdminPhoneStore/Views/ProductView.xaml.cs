using System.Windows.Controls;

namespace AdminPhoneStore.Views
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {
        public ProductView()
        {
            InitializeComponent();
            // TODO: Khi có ProductViewModel, inject vào đây
            // DataContext = ServiceLocator.GetService<ProductViewModel>();
        }
    }
}
