using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for OrderView.xaml
    /// </summary>
    public partial class OrderView : UserControl
    {
        public OrderView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.GetService<OrderViewModel>();
        }

        private void OrdersDataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is DataGrid dg && dg.Parent is ScrollViewer sv)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void OrdersDataGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                dg.Focus();
            }
        }
    }
}
