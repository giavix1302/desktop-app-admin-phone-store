using System.Windows;
using System.Windows.Controls;
using AdminPhoneStore.ViewModels.Dialogs;

namespace AdminPhoneStore.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for ToastView.xaml
    /// </summary>
    public partial class ToastView : UserControl
    {
        public ToastView()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ToastViewModel viewModel)
            {
                viewModel.Close();
            }
        }
    }
}
