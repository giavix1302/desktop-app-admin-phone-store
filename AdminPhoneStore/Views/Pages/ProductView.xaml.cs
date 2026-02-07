using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.Models;
using AdminPhoneStore.ViewModels.Pages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
                        // Delay một chút để đảm bảo UI đã update
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            SyncListBoxSelection();
                        }), System.Windows.Threading.DispatcherPriority.Loaded);
                    }
                    else if (args.PropertyName == nameof(ProductViewModel.Colors))
                    {
                        // Khi Colors được load, sync lại selection nếu đang edit
                        if (_viewModel.IsEditMode && _viewModel.SelectedColorIds.Count > 0)
                        {
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                SyncListBoxSelection();
                            }), System.Windows.Threading.DispatcherPriority.Loaded);
                        }
                    }
                };
                
                // Initial sync
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    SyncListBoxSelection();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private void ColorsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || ColorsListBox == null) return;

            // Sync ListBox selection với ViewModel.SelectedColors
            var selectedColors = new ObservableCollection<AdminPhoneStore.Models.Color>();
            foreach (AdminPhoneStore.Models.Color color in ColorsListBox.SelectedItems)
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

        private void ProductsDataGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            // Focus DataGrid khi mouse vào để có thể scroll
            if (sender is DataGrid dataGrid)
            {
                dataGrid.Focus();
            }
        }

        private void ProductsDataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Đảm bảo DataGrid có thể scroll bằng mouse wheel
            if (sender is DataGrid dataGrid)
            {
                var scrollViewer = GetScrollViewer(dataGrid);
                if (scrollViewer != null)
                {
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 3.0);
                    e.Handled = true;
                }
            }
        }

        private static ScrollViewer? GetScrollViewer(DependencyObject element)
        {
            if (element is ScrollViewer scrollViewer)
            {
                return scrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var child = VisualTreeHelper.GetChild(element, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
