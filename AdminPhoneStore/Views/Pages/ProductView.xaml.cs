using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.Models;
using AdminPhoneStore.ViewModels.Pages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    /// <summary>
    /// Interaction logic for ProductView.xaml
    /// </summary>
    public partial class ProductView : UserControl
    {
        private ProductViewModel? _viewModel;

        public ProductView()
        {
            InitializeComponent();
            _viewModel = ServiceLocator.GetService<ProductViewModel>();
            DataContext = _viewModel;
            Loaded += ProductView_Loaded;
        }

        private void ProductView_Loaded(object sender, RoutedEventArgs e)
        {
            // Sync SelectedColors với ListBox khi ViewModel thay đổi SelectedColors
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(ProductViewModel.SelectedColors))
                    {
                        SyncListBoxSelection();
                    }
                };
            }
        }

        private void ColorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || ColorsListBox == null) return;

            // Sync ListBox selection với ViewModel.SelectedColors
            var selectedColors = new ObservableCollection<Color>();
            foreach (Color color in ColorsListBox.SelectedItems)
            {
                selectedColors.Add(color);
            }
            _viewModel.SelectedColors = selectedColors;
        }

        private void SyncListBoxSelection()
        {
            if (_viewModel == null || ColorsListBox == null) return;

            // Prevent recursive updates
            ColorsListBox.SelectionChanged -= ColorsListBox_SelectionChanged;

            ColorsListBox.SelectedItems.Clear();
            foreach (var color in _viewModel.SelectedColors)
            {
                ColorsListBox.SelectedItems.Add(color);
            }

            ColorsListBox.SelectionChanged += ColorsListBox_SelectionChanged;
        }
    }
}
