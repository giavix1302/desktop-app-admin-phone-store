using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Pages;
using LiveChartsCore.SkiaSharpView.WPF;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace AdminPhoneStore.Views.Pages
{
    public partial class DashboardView : UserControl
    {
        private CartesianChart? _revenueChart;
        private PieChart? _pieChart;
        private DashboardViewModel? _vm;

        public DashboardView()
        {
            InitializeComponent();
            DataContext = ServiceLocator.GetService<DashboardViewModel>();
        }

        private void DashboardView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_vm != null)
                _vm.PropertyChanged -= Vm_PropertyChanged;

            _vm = DataContext as DashboardViewModel;

            // Create CartesianChart
            _revenueChart = new CartesianChart();
            _revenueChart.SetBinding(CartesianChart.SeriesProperty,
                new System.Windows.Data.Binding(nameof(DashboardViewModel.RevenueSeries)));
            _revenueChart.SetBinding(CartesianChart.XAxesProperty,
                new System.Windows.Data.Binding(nameof(DashboardViewModel.RevenueXAxes)));
            _revenueChart.SetBinding(CartesianChart.YAxesProperty,
                new System.Windows.Data.Binding(nameof(DashboardViewModel.RevenueYAxes)));
            RevenueChartHost.Child = _revenueChart;

            // Create PieChart
            _pieChart = new PieChart
            {
                InitialRotation = -90
            };
            _pieChart.SetBinding(PieChart.SeriesProperty,
                new System.Windows.Data.Binding(nameof(DashboardViewModel.OrderStatusSeries)));
            PieChartHost.Child = _pieChart;

            // Bind legend
            StatusLegend.SetBinding(ItemsControl.ItemsSourceProperty,
                new System.Windows.Data.Binding(nameof(DashboardViewModel.OrderStatusSeries)));

            if (_vm != null)
                _vm.PropertyChanged += Vm_PropertyChanged;
        }

        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Charts update automatically via binding
        }
    }
}
