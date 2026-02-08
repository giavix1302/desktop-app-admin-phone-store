using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class OrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IDialogService _dialogService;

        // Orders list
        private ObservableCollection<Order> _orders = new();
        private Order? _selectedOrder;
        private OrderDetail? _orderDetail;
        private bool _isLoading;
        private bool _showOrderDetail;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalItems;
        private int _totalPages;

        // Filters
        private OrderStatus? _selectedStatus;
        private PaymentStatus? _selectedPaymentStatus;
        private PaymentMethod? _selectedPaymentMethod;
        private string _searchOrderNumber = string.Empty;
        private DateTime? _filterFromDate;
        private DateTime? _filterToDate;
        private string _sortBy = "createdAt";
        private string _sortDir = "desc";

        // Order Detail - Status Update
        private OrderStatus? _newStatus;

        // Order Detail - Payment Update
        private PaymentStatus? _newPaymentStatus;
        private PaymentMethod? _newPaymentMethod;

        // Tracking
        private ObservableCollection<OrderTracking> _trackings = new();
        private OrderTracking? _selectedTracking;
        private bool _showTrackingForm;
        private bool _isEditTracking;
        private OrderStatus _trackingStatus;
        private string? _trackingLocation;
        private string? _trackingDescription;
        private string? _trackingNote;
        private string? _trackingNumber;
        private string? _shippingPattern;
        private DateTime? _estimatedDelivery;

        // Stats
        private OrderStats? _orderStats;

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged();
            }
        }

        public Order? SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _ = LoadOrderDetailAsync(value.Id);
                }
            }
        }

        public OrderDetail? OrderDetail
        {
            get => _orderDetail;
            set
            {
                _orderDetail = value;
                OnPropertyChanged();
                if (value != null)
                {
                    Trackings = new ObservableCollection<OrderTracking>(value.Trackings);
                    // Set default values for status update
                    NewStatus = value.Status;
                    NewPaymentStatus = value.PaymentStatus;
                    NewPaymentMethod = value.PaymentMethod;
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public bool ShowOrderDetail
        {
            get => _showOrderDetail;
            set
            {
                _showOrderDetail = value;
                OnPropertyChanged();
            }
        }

        // Pagination
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                OnPropertyChanged();
            }
        }

        public int TotalItems
        {
            get => _totalItems;
            set
            {
                _totalItems = value;
                OnPropertyChanged();
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanGoToNextPage));
            }
        }

        public bool CanGoToPreviousPage => CurrentPage > 1;
        public bool CanGoToNextPage => CurrentPage < TotalPages;

        // Filters
        public OrderStatus? SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }

        public PaymentStatus? SelectedPaymentStatus
        {
            get => _selectedPaymentStatus;
            set
            {
                _selectedPaymentStatus = value;
                OnPropertyChanged();
            }
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged();
            }
        }

        public string SearchOrderNumber
        {
            get => _searchOrderNumber;
            set
            {
                _searchOrderNumber = value;
                OnPropertyChanged();
            }
        }

        public DateTime? FilterFromDate
        {
            get => _filterFromDate;
            set
            {
                _filterFromDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime? FilterToDate
        {
            get => _filterToDate;
            set
            {
                _filterToDate = value;
                OnPropertyChanged();
            }
        }

        public string SortBy
        {
            get => _sortBy;
            set
            {
                _sortBy = value;
                OnPropertyChanged();
            }
        }

        public string SortDir
        {
            get => _sortDir;
            set
            {
                _sortDir = value;
                OnPropertyChanged();
            }
        }

        // Order Detail - Status
        public OrderStatus? NewStatus
        {
            get => _newStatus;
            set
            {
                _newStatus = value;
                OnPropertyChanged();
            }
        }

        // Order Detail - Payment
        public PaymentStatus? NewPaymentStatus
        {
            get => _newPaymentStatus;
            set
            {
                _newPaymentStatus = value;
                OnPropertyChanged();
            }
        }

        public PaymentMethod? NewPaymentMethod
        {
            get => _newPaymentMethod;
            set
            {
                _newPaymentMethod = value;
                OnPropertyChanged();
            }
        }

        // Tracking
        public ObservableCollection<OrderTracking> Trackings
        {
            get => _trackings;
            set
            {
                _trackings = value;
                OnPropertyChanged();
            }
        }

        public OrderTracking? SelectedTracking
        {
            get => _selectedTracking;
            set
            {
                _selectedTracking = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTrackingForm
        {
            get => _showTrackingForm;
            set
            {
                _showTrackingForm = value;
                OnPropertyChanged();
            }
        }

        public bool IsEditTracking
        {
            get => _isEditTracking;
            set
            {
                _isEditTracking = value;
                OnPropertyChanged();
            }
        }

        public OrderStatus TrackingStatus
        {
            get => _trackingStatus;
            set
            {
                _trackingStatus = value;
                OnPropertyChanged();
            }
        }

        public string? TrackingLocation
        {
            get => _trackingLocation;
            set
            {
                _trackingLocation = value;
                OnPropertyChanged();
            }
        }

        public string? TrackingDescription
        {
            get => _trackingDescription;
            set
            {
                _trackingDescription = value;
                OnPropertyChanged();
            }
        }

        public string? TrackingNote
        {
            get => _trackingNote;
            set
            {
                _trackingNote = value;
                OnPropertyChanged();
            }
        }

        public string? TrackingNumber
        {
            get => _trackingNumber;
            set
            {
                _trackingNumber = value;
                OnPropertyChanged();
            }
        }

        public string? ShippingPattern
        {
            get => _shippingPattern;
            set
            {
                _shippingPattern = value;
                OnPropertyChanged();
            }
        }

        public DateTime? EstimatedDelivery
        {
            get => _estimatedDelivery;
            set
            {
                _estimatedDelivery = value;
                OnPropertyChanged();
            }
        }

        public OrderStats? OrderStats
        {
            get => _orderStats;
            set
            {
                _orderStats = value;
                OnPropertyChanged();
            }
        }

        // Enum collections for ComboBox
        public List<OrderStatus> OrderStatuses { get; } = Enum.GetValues<OrderStatus>().ToList();
        public List<PaymentStatus> PaymentStatuses { get; } = Enum.GetValues<PaymentStatus>().ToList();
        public List<PaymentMethod> PaymentMethods { get; } = Enum.GetValues<PaymentMethod>().ToList();

        // Commands
        public RelayCommand LoadOrdersCommand { get; }
        public RelayCommand LoadOrderDetailCommand { get; }
        public RelayCommand ApplyFiltersCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand UpdateStatusCommand { get; }
        public RelayCommand UpdatePaymentCommand { get; }
        public RelayCommand AddTrackingCommand { get; }
        public RelayCommand EditTrackingCommand { get; }
        public RelayCommand SaveTrackingCommand { get; }
        public RelayCommand CancelTrackingCommand { get; }
        public RelayCommand DeleteTrackingCommand { get; }
        public RelayCommand LoadStatsCommand { get; }
        public RelayCommand CloseOrderDetailCommand { get; }

        public OrderViewModel(
            IOrderService orderService,
            IDialogService dialogService)
        {
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadOrdersCommand = new RelayCommand(async () => await LoadOrdersAsync());
            LoadOrderDetailCommand = new RelayCommand(async () =>
            {
                if (SelectedOrder != null)
                {
                    await LoadOrderDetailAsync(SelectedOrder.Id);
                }
            }, () => SelectedOrder != null);
            ApplyFiltersCommand = new RelayCommand(async () =>
            {
                CurrentPage = 1;
                await LoadOrdersAsync();
            });
            ClearFiltersCommand = new RelayCommand(() => ClearFilters());
            PreviousPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToPreviousPage)
                {
                    CurrentPage--;
                    await LoadOrdersAsync();
                }
            }, () => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToNextPage)
                {
                    CurrentPage++;
                    await LoadOrdersAsync();
                }
            }, () => CanGoToNextPage);
            UpdateStatusCommand = new RelayCommand(async () => await UpdateStatusAsync(), () => OrderDetail != null && NewStatus.HasValue);
            UpdatePaymentCommand = new RelayCommand(async () => await UpdatePaymentAsync(), () => OrderDetail != null && NewPaymentStatus.HasValue);
            AddTrackingCommand = new RelayCommand(() => ShowAddTrackingForm(), () => OrderDetail != null);
            EditTrackingCommand = new RelayCommand(() =>
            {
                if (SelectedTracking != null)
                {
                    LoadTrackingForEdit(SelectedTracking);
                }
            }, () => SelectedTracking != null);
            SaveTrackingCommand = new RelayCommand(async () => await SaveTrackingAsync());
            CancelTrackingCommand = new RelayCommand(() => CancelTrackingForm());
            DeleteTrackingCommand = new RelayCommand(async () => await DeleteTrackingAsync(), () => SelectedTracking != null);
            LoadStatsCommand = new RelayCommand(async () => await LoadStatsAsync());
            CloseOrderDetailCommand = new RelayCommand(() => CloseOrderDetail());

            // Load data khi khởi tạo
            _ = LoadOrdersAsync();
            _ = LoadStatsAsync();
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true;
                var filter = new OrderFilterRequest
                {
                    Status = SelectedStatus,
                    PaymentStatus = SelectedPaymentStatus,
                    PaymentMethod = SelectedPaymentMethod,
                    OrderNumber = string.IsNullOrWhiteSpace(SearchOrderNumber) ? null : SearchOrderNumber,
                    From = FilterFromDate,
                    To = FilterToDate,
                    SortBy = SortBy,
                    SortDir = SortDir,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var response = await _orderService.GetOrdersAsync(filter);
                
                // Debug logging
                System.Diagnostics.Debug.WriteLine($"LoadOrdersAsync: Response received. TotalItems: {response?.TotalItems}, Items count: {response?.Items?.Count ?? 0}");
                
                if (response != null && response.Items != null)
                {
                    System.Diagnostics.Debug.WriteLine($"LoadOrdersAsync: Adding {response.Items.Count} orders to collection");
                    Orders = new ObservableCollection<Order>(response.Items);
                    TotalItems = response.TotalItems;
                    TotalPages = response.TotalPages;
                    
                    System.Diagnostics.Debug.WriteLine($"LoadOrdersAsync: Orders collection now has {Orders.Count} items");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadOrdersAsync: Response or Items is null");
                    Orders = new ObservableCollection<Order>();
                    TotalItems = 0;
                    TotalPages = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadOrdersAsync Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                _dialogService.ShowError($"Lỗi khi tải danh sách đơn hàng: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadOrderDetailAsync(long orderId)
        {
            try
            {
                IsLoading = true;
                var detail = await _orderService.GetOrderByIdAsync(orderId);
                OrderDetail = detail;
                ShowOrderDetail = detail != null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải chi tiết đơn hàng: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearFilters()
        {
            SelectedStatus = null;
            SelectedPaymentStatus = null;
            SelectedPaymentMethod = null;
            SearchOrderNumber = string.Empty;
            FilterFromDate = null;
            FilterToDate = null;
            SortBy = "createdAt";
            SortDir = "desc";
            CurrentPage = 1;
            _ = LoadOrdersAsync();
        }

        private async Task UpdateStatusAsync()
        {
            if (OrderDetail == null || !NewStatus.HasValue) return;

            try
            {
                if (NewStatus.Value == OrderStatus.CANCELLED)
                {
                    _dialogService.ShowError("Admin không được đặt trạng thái CANCELLED", "Lỗi");
                    return;
                }

                bool confirmed = _dialogService.ShowConfirmation(
                    $"Bạn có chắc muốn cập nhật trạng thái đơn hàng thành '{NewStatus.Value}'?",
                    "Xác nhận",
                    "Cập nhật",
                    "Hủy"
                );

                if (!confirmed) return;

                IsLoading = true;
                var request = new UpdateOrderStatusRequest { Status = NewStatus.Value };
                var success = await _orderService.UpdateOrderStatusAsync(OrderDetail.Id, request);
                if (success)
                {
                    _dialogService.ShowSuccess("Cập nhật trạng thái thành công!");
                    await LoadOrderDetailAsync(OrderDetail.Id);
                    await LoadOrdersAsync();
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi cập nhật trạng thái: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdatePaymentAsync()
        {
            if (OrderDetail == null || !NewPaymentStatus.HasValue) return;

            try
            {
                bool confirmed = _dialogService.ShowConfirmation(
                    $"Bạn có chắc muốn cập nhật thanh toán đơn hàng?",
                    "Xác nhận",
                    "Cập nhật",
                    "Hủy"
                );

                if (!confirmed) return;

                IsLoading = true;
                var request = new UpdateOrderPaymentRequest
                {
                    PaymentStatus = NewPaymentStatus.Value,
                    PaymentMethod = NewPaymentMethod
                };
                var success = await _orderService.UpdateOrderPaymentAsync(OrderDetail.Id, request);
                if (success)
                {
                    _dialogService.ShowSuccess("Cập nhật thanh toán thành công!");
                    await LoadOrderDetailAsync(OrderDetail.Id);
                    await LoadOrdersAsync();
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi cập nhật thanh toán: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowAddTrackingForm()
        {
            SelectedTracking = null;
            IsEditTracking = false;
            TrackingStatus = OrderDetail?.Status ?? OrderStatus.PENDING;
            TrackingLocation = string.Empty;
            TrackingDescription = string.Empty;
            TrackingNote = string.Empty;
            TrackingNumber = string.Empty;
            ShippingPattern = string.Empty;
            EstimatedDelivery = null;
            ShowTrackingForm = true;
        }

        private void LoadTrackingForEdit(OrderTracking tracking)
        {
            TrackingStatus = tracking.Status;
            TrackingLocation = tracking.Location;
            TrackingDescription = tracking.Description;
            TrackingNote = tracking.Note;
            TrackingNumber = tracking.TrackingNumber;
            ShippingPattern = tracking.ShippingPattern;
            EstimatedDelivery = tracking.EstimatedDelivery;
            IsEditTracking = true;
            ShowTrackingForm = true;
        }

        private void CancelTrackingForm()
        {
            ShowTrackingForm = false;
            SelectedTracking = null;
            IsEditTracking = false;
        }

        private async Task SaveTrackingAsync()
        {
            if (OrderDetail == null) return;

            try
            {
                IsLoading = true;
                var request = new AddTrackingRequest
                {
                    Status = TrackingStatus,
                    Location = string.IsNullOrWhiteSpace(TrackingLocation) ? null : TrackingLocation,
                    Description = string.IsNullOrWhiteSpace(TrackingDescription) ? null : TrackingDescription,
                    Note = string.IsNullOrWhiteSpace(TrackingNote) ? null : TrackingNote,
                    TrackingNumber = string.IsNullOrWhiteSpace(TrackingNumber) ? null : TrackingNumber,
                    ShippingPattern = string.IsNullOrWhiteSpace(ShippingPattern) ? null : ShippingPattern,
                    EstimatedDelivery = EstimatedDelivery
                };

                if (IsEditTracking && SelectedTracking != null)
                {
                    var success = await _orderService.UpdateTrackingAsync(OrderDetail.Id, SelectedTracking.Id, request);
                    if (success)
                    {
                        _dialogService.ShowSuccess("Cập nhật tracking thành công!");
                        await LoadOrderDetailAsync(OrderDetail.Id);
                        CancelTrackingForm();
                    }
                }
                else
                {
                    var response = await _orderService.AddTrackingAsync(OrderDetail.Id, request);
                    if (response != null)
                    {
                        _dialogService.ShowSuccess("Thêm tracking thành công!");
                        await LoadOrderDetailAsync(OrderDetail.Id);
                        CancelTrackingForm();
                    }
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi lưu tracking: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteTrackingAsync()
        {
            if (OrderDetail == null || SelectedTracking == null) return;

            try
            {
                bool confirmed = _dialogService.ShowConfirmation(
                    "Bạn có chắc muốn xóa tracking này?",
                    "Xác nhận xóa",
                    "Xóa",
                    "Hủy"
                );

                if (!confirmed) return;

                IsLoading = true;
                var success = await _orderService.DeleteTrackingAsync(OrderDetail.Id, SelectedTracking.Id);
                if (success)
                {
                    _dialogService.ShowSuccess("Xóa tracking thành công!");
                    await LoadOrderDetailAsync(OrderDetail.Id);
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa tracking: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadStatsAsync()
        {
            try
            {
                var stats = await _orderService.GetOrderStatsAsync(FilterFromDate, FilterToDate);
                OrderStats = stats;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi");
            }
        }

        private void CloseOrderDetail()
        {
            ShowOrderDetail = false;
            OrderDetail = null;
            SelectedOrder = null;
            ShowTrackingForm = false;
        }
    }
}
