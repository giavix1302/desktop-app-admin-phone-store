using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Collections.ObjectModel;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class ReviewViewModel : BaseViewModel
    {
        private readonly IReviewService _reviewService;
        private readonly IDialogService _dialogService;

        // Reviews list
        private ObservableCollection<Review> _reviews = new();
        private Review? _selectedReview;
        private ReviewDetail? _reviewDetail;
        private bool _isLoading;
        private bool _showReviewDetail;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalItems;
        private int _totalPages;

        // Filters
        private string _filterProductId = string.Empty;
        private string _filterUserId = string.Empty;
        private int? _selectedRating;
        private DateTime? _filterFromDate;
        private DateTime? _filterToDate;
        private string _sortBy = "createdAt";
        private string _sortDir = "desc";

        // Stats
        private ReviewStats? _reviewStats;

        public ObservableCollection<Review> Reviews
        {
            get => _reviews;
            set { _reviews = value; OnPropertyChanged(); }
        }

        public Review? SelectedReview
        {
            get => _selectedReview;
            set
            {
                _selectedReview = value;
                OnPropertyChanged();
                if (value != null)
                    _ = LoadReviewDetailAsync(value.Id);
            }
        }

        public ReviewDetail? ReviewDetail
        {
            get => _reviewDetail;
            set { _reviewDetail = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool ShowReviewDetail
        {
            get => _showReviewDetail;
            set { _showReviewDetail = value; OnPropertyChanged(); }
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
            set { _pageSize = value; OnPropertyChanged(); }
        }

        public int TotalItems
        {
            get => _totalItems;
            set { _totalItems = value; OnPropertyChanged(); }
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
        public string FilterProductId
        {
            get => _filterProductId;
            set { _filterProductId = value; OnPropertyChanged(); }
        }

        public string FilterUserId
        {
            get => _filterUserId;
            set { _filterUserId = value; OnPropertyChanged(); }
        }

        public int? SelectedRating
        {
            get => _selectedRating;
            set { _selectedRating = value; OnPropertyChanged(); }
        }

        public DateTime? FilterFromDate
        {
            get => _filterFromDate;
            set { _filterFromDate = value; OnPropertyChanged(); }
        }

        public DateTime? FilterToDate
        {
            get => _filterToDate;
            set { _filterToDate = value; OnPropertyChanged(); }
        }

        public string SortBy
        {
            get => _sortBy;
            set { _sortBy = value; OnPropertyChanged(); }
        }

        public string SortDir
        {
            get => _sortDir;
            set { _sortDir = value; OnPropertyChanged(); }
        }

        public ReviewStats? ReviewStats
        {
            get => _reviewStats;
            set { _reviewStats = value; OnPropertyChanged(); }
        }

        // Enum/fixed lists
        public List<int?> RatingOptions { get; } = new() { null, 1, 2, 3, 4, 5 };
        public List<string> SortByOptions { get; } = new() { "createdAt", "rating", "updatedAt" };
        public List<string> SortDirOptions { get; } = new() { "desc", "asc" };

        // Commands
        public RelayCommand LoadReviewsCommand { get; }
        public RelayCommand ApplyFiltersCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand DeleteReviewCommand { get; }
        public RelayCommand LoadStatsCommand { get; }
        public RelayCommand CloseReviewDetailCommand { get; }

        public ReviewViewModel(
            IReviewService reviewService,
            IDialogService dialogService)
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadReviewsCommand = new RelayCommand(async () => await LoadReviewsAsync());
            ApplyFiltersCommand = new RelayCommand(async () =>
            {
                CurrentPage = 1;
                await LoadReviewsAsync();
            });
            ClearFiltersCommand = new RelayCommand(() => ClearFilters());
            PreviousPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToPreviousPage)
                {
                    CurrentPage--;
                    await LoadReviewsAsync();
                }
            }, () => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToNextPage)
                {
                    CurrentPage++;
                    await LoadReviewsAsync();
                }
            }, () => CanGoToNextPage);
            DeleteReviewCommand = new RelayCommand(async () => await DeleteReviewAsync(), () => SelectedReview != null);
            LoadStatsCommand = new RelayCommand(async () => await LoadStatsAsync());
            CloseReviewDetailCommand = new RelayCommand(() => CloseReviewDetail());

            _ = LoadReviewsAsync();
            _ = LoadStatsAsync();
        }

        private async Task LoadReviewsAsync()
        {
            try
            {
                IsLoading = true;
                var filter = new ReviewFilterRequest
                {
                    ProductId = long.TryParse(FilterProductId, out var pid) ? pid : null,
                    UserId = long.TryParse(FilterUserId, out var uid) ? uid : null,
                    Rating = SelectedRating,
                    From = FilterFromDate,
                    To = FilterToDate,
                    SortBy = SortBy,
                    SortDir = SortDir,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var response = await _reviewService.GetReviewsAsync(filter);
                if (response?.Items != null)
                {
                    Reviews = new ObservableCollection<Review>(response.Items);
                    TotalItems = response.TotalItems;
                    TotalPages = response.TotalPages;
                }
                else
                {
                    Reviews = new ObservableCollection<Review>();
                    TotalItems = 0;
                    TotalPages = 0;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách review: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadReviewDetailAsync(long reviewId)
        {
            try
            {
                IsLoading = true;
                var detail = await _reviewService.GetReviewByIdAsync(reviewId);
                ReviewDetail = detail;
                ShowReviewDetail = detail != null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải chi tiết review: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearFilters()
        {
            FilterProductId = string.Empty;
            FilterUserId = string.Empty;
            SelectedRating = null;
            FilterFromDate = null;
            FilterToDate = null;
            SortBy = "createdAt";
            SortDir = "desc";
            CurrentPage = 1;
            _ = LoadReviewsAsync();
        }

        private async Task DeleteReviewAsync()
        {
            if (SelectedReview == null) return;

            try
            {
                bool confirmed = _dialogService.ShowConfirmation(
                    $"Bạn có chắc muốn xóa review #{SelectedReview.Id} của '{SelectedReview.UserName}'?",
                    "Xác nhận xóa",
                    "Xóa",
                    "Hủy"
                );

                if (!confirmed) return;

                IsLoading = true;
                var success = await _reviewService.DeleteReviewAsync(SelectedReview.Id);
                if (success)
                {
                    _dialogService.ShowSuccess("Xóa review thành công!");
                    CloseReviewDetail();
                    await LoadReviewsAsync();
                    await LoadStatsAsync();
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa review: {ex.Message}", "Lỗi");
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
                var stats = await _reviewService.GetReviewStatsAsync(FilterFromDate, FilterToDate);
                ReviewStats = stats;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi");
            }
        }

        private void CloseReviewDetail()
        {
            ShowReviewDetail = false;
            ReviewDetail = null;
            SelectedReview = null;
        }
    }
}
