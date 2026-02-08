using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using System.Windows;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public UserView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.GetService<UserViewModel>();
        }

        private void UsersDataGrid_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is DataGrid dg && dg.Parent is ScrollViewer sv)
            {
                sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void UsersDataGrid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                dg.Focus();
            }
        }

        private void EnabledFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item && DataContext is ViewModels.Pages.UserViewModel viewModel)
            {
                var tag = item.Tag;
                if (tag == null)
                {
                    viewModel.EnabledFilter = null;
                }
                else if (tag is string strTag)
                {
                    if (bool.TryParse(strTag, out bool boolValue))
                    {
                        viewModel.EnabledFilter = boolValue;
                    }
                }
            }
        }
    }
}
