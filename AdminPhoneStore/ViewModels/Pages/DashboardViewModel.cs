using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models.Dashboard;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDashboardService _dashboardService;
        private readonly IDialogService _dialogService;

        // KPI Cards
        private decimal _revenueThisMonth;
        private string _revenueChangeText = "0%";
        private bool _revenueIsPositive = true;
        private int _ordersThisMonth;
        private int _totalUsers;
        private int _newUsersThisMonth;
        private int _totalActiveProducts;
        private int _lowStockCount;

        // Chart
        private ISeries[] _revenueSeries = Array.Empty<ISeries>();
        private Axis[] _revenueXAxes = Array.Empty<Axis>();
        private Axis[] _revenueYAxes = Array.Empty<Axis>();
        private string _selectedPeriod = "30d";
        private IEnumerable<ISeries> _orderStatusSeries = Array.Empty<ISeries>();
        private int _totalOrders;

        // State
        private bool _isLoading;

        public decimal RevenueThisMonth { get => _revenueThisMonth; set { _revenueThisMonth = value; OnPropertyChanged(); } }
        public string RevenueChangeText { get => _revenueChangeText; set { _revenueChangeText = value; OnPropertyChanged(); } }
        public bool RevenueIsPositive { get => _revenueIsPositive; set { _revenueIsPositive = value; OnPropertyChanged(); } }
        public int OrdersThisMonth { get => _ordersThisMonth; set { _ordersThisMonth = value; OnPropertyChanged(); } }
        public int TotalUsers { get => _totalUsers; set { _totalUsers = value; OnPropertyChanged(); } }
        public int NewUsersThisMonth { get => _newUsersThisMonth; set { _newUsersThisMonth = value; OnPropertyChanged(); } }
        public int TotalActiveProducts { get => _totalActiveProducts; set { _totalActiveProducts = value; OnPropertyChanged(); } }
        public int LowStockCount { get => _lowStockCount; set { _lowStockCount = value; OnPropertyChanged(); } }

        public ISeries[] RevenueSeries { get => _revenueSeries; set { _revenueSeries = value; OnPropertyChanged(); } }
        public Axis[] RevenueXAxes { get => _revenueXAxes; set { _revenueXAxes = value; OnPropertyChanged(); } }
        public Axis[] RevenueYAxes { get => _revenueYAxes; set { _revenueYAxes = value; OnPropertyChanged(); } }
        public string SelectedPeriod { get => _selectedPeriod; set { _selectedPeriod = value; OnPropertyChanged(); } }
        public IEnumerable<ISeries> OrderStatusSeries { get => _orderStatusSeries; set { _orderStatusSeries = value; OnPropertyChanged(); } }
        public int TotalOrders { get => _totalOrders; set { _totalOrders = value; OnPropertyChanged(); } }

        public bool IsLoading { get => _isLoading; set { _isLoading = value; OnPropertyChanged(); } }

        public RelayCommand RefreshCommand { get; }
        public RelayCommand<string> ChangePeriodCommand { get; }

        public DashboardViewModel(IDashboardService dashboardService, IDialogService dialogService)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            RefreshCommand = new RelayCommand(() => { _ = LoadAsync(); });
            ChangePeriodCommand = new RelayCommand<string>(period =>
            {
                if (period != null && period != SelectedPeriod)
                {
                    SelectedPeriod = period;
                    _ = LoadRevenueChartAsync();
                }
            });

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var summaryTask = _dashboardService.GetSummaryAsync();
                var orderStatusTask = _dashboardService.GetOrderStatusChartAsync();

                await Task.WhenAll(summaryTask, orderStatusTask);

                var summary = summaryTask.Result;
                var orderStatus = orderStatusTask.Result;

                if (summary != null)
                    ApplySummary(summary);

                if (orderStatus != null)
                    ApplyOrderStatusChart(orderStatus);

                await LoadRevenueChartAsync();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi tải dữ liệu dashboard: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadRevenueChartAsync()
        {
            try
            {
                var data = await _dashboardService.GetRevenueChartAsync(SelectedPeriod);
                if (data != null)
                    ApplyRevenueChart(data);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi tải biểu đồ doanh thu: {ex.Message}", "Lỗi");
            }
        }

        private void ApplySummary(DashboardSummary summary)
        {
            RevenueThisMonth = summary.RevenueThisMonth;
            OrdersThisMonth = summary.OrdersThisMonth;
            TotalUsers = summary.TotalUsers;
            NewUsersThisMonth = summary.NewUsersThisMonth;
            TotalActiveProducts = summary.TotalActiveProducts;
            LowStockCount = summary.LowStockCount;

            var pct = summary.RevenueChangePercent;
            RevenueIsPositive = pct >= 0;
            RevenueChangeText = pct >= 0 ? $"+{pct:F1}%" : $"{pct:F1}%";
        }

        private void ApplyRevenueChart(RevenueChartData data)
        {
            var points = data.Data;
            var values = points.Select(p => (double)p.Revenue).ToArray();
            var labels = points.Select(p => p.Label).ToArray();

            RevenueSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = values,
                    Fill = new LinearGradientPaint(
                        new SKColor(104, 90, 255, 40),
                        new SKColor(104, 90, 255, 0)),
                    Stroke = new SolidColorPaint(SKColor.Parse("#685AFF")) { StrokeThickness = 2 },
                    GeometryStroke = new SolidColorPaint(SKColor.Parse("#685AFF")) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.White),
                    GeometrySize = 8,
                    Name = "Doanh thu"
                }
            };

            RevenueXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = labels,
                    LabelsRotation = -30,
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B"))
                }
            };

            RevenueYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = v => v >= 1_000_000 ? $"{v / 1_000_000:F1}M" : $"{v / 1_000:F0}K",
                    TextSize = 11,
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#64748B"))
                }
            };
        }

        private void ApplyOrderStatusChart(OrderStatusChartData data)
        {
            TotalOrders = data.Total;

            var colors = new Dictionary<string, string>
            {
                ["PENDING"]    = "#F59E0B",
                ["PROCESSING"] = "#3B82F6",
                ["SHIPPED"]    = "#8B5CF6",
                ["DELIVERED"]  = "#10B981",
                ["CANCELLED"]  = "#EF4444"
            };

            OrderStatusSeries = data.Data.Select(s =>
            {
                var color = colors.TryGetValue(s.Status, out var c) ? c : "#94A3B8";
                return (ISeries)new PieSeries<double>
                {
                    Values = new[] { (double)s.Count },
                    Name = s.Status,
                    Fill = new SolidColorPaint(SKColor.Parse(color)),
                    InnerRadius = 60,
                    DataLabelsSize = 0
                };
            }).ToArray();
        }
    }
}
