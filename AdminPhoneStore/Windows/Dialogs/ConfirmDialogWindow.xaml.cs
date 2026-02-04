using System.Windows;
using AdminPhoneStore.ViewModels.Dialogs;

namespace AdminPhoneStore.Windows.Dialogs
{
    /// <summary>
    /// Interaction logic for ConfirmDialogWindow.xaml
    /// </summary>
    public partial class ConfirmDialogWindow : Window
    {
        private bool _result = false;

        public bool Result => _result;

        public ConfirmDialogWindow(ConfirmDialogViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.Result += (s, confirmed) =>
            {
                _result = confirmed;
                DialogResult = true;
                Close();
            };
        }
    }
}
