namespace AdminPhoneStore.Models.Dashboard
{
    public class DashboardSummary
    {
        public decimal RevenueThisMonth { get; set; }
        public decimal RevenueLastMonth { get; set; }
        public double RevenueChangePercent { get; set; }
        public int OrdersThisMonth { get; set; }
        public Dictionary<string, int> OrdersByStatus { get; set; } = new();
        public int TotalUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int TotalActiveProducts { get; set; }
        public int LowStockCount { get; set; }
    }

    public class RevenueChartPoint
    {
        public string Label { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class RevenueChartData
    {
        public string Period { get; set; } = string.Empty;
        public List<RevenueChartPoint> Data { get; set; } = new();
    }

    public class OrderStatusPoint
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percent { get; set; }
    }

    public class OrderStatusChartData
    {
        public int Total { get; set; }
        public List<OrderStatusPoint> Data { get; set; } = new();
    }
}
