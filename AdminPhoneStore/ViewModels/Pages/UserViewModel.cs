using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Collections.ObjectModel;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class UserViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;

        // Users list
        private ObservableCollection<User> _users = new();
        private User? _selectedUser;
        private UserDetail? _userDetail;
        private bool _isLoading;
        private bool _showUserDetail;

        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalItems;
        private int _totalPages;

        // Filters
        private string _searchEmail = string.Empty;
        private string _searchFullName = string.Empty;
        private bool? _enabledFilter;
        private DateTime? _filterFromDate;
        private DateTime? _filterToDate;
        private string _sortBy = "createdAt";
        private string _sortDir = "desc";

        public ObservableCollection<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                OnPropertyChanged();
                if (value != null)
                {
                    _ = LoadUserDetailAsync(value.Id);
                }
            }
        }

        public UserDetail? UserDetail
        {
            get => _userDetail;
            set
            {
                _userDetail = value;
                OnPropertyChanged();
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

        public bool ShowUserDetail
        {
            get => _showUserDetail;
            set
            {
                _showUserDetail = value;
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
        public string SearchEmail
        {
            get => _searchEmail;
            set
            {
                _searchEmail = value;
                OnPropertyChanged();
            }
        }

        public string SearchFullName
        {
            get => _searchFullName;
            set
            {
                _searchFullName = value;
                OnPropertyChanged();
            }
        }

        public bool? EnabledFilter
        {
            get => _enabledFilter;
            set
            {
                _enabledFilter = value;
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

        // Commands
        public RelayCommand LoadUsersCommand { get; }
        public RelayCommand LoadUserDetailCommand { get; }
        public RelayCommand ApplyFiltersCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public RelayCommand PreviousPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand CloseUserDetailCommand { get; }

        public UserViewModel(
            IUserService userService,
            IDialogService dialogService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadUsersCommand = new RelayCommand(async () => await LoadUsersAsync());
            LoadUserDetailCommand = new RelayCommand(async () =>
            {
                if (SelectedUser != null)
                {
                    await LoadUserDetailAsync(SelectedUser.Id);
                }
            }, () => SelectedUser != null);
            ApplyFiltersCommand = new RelayCommand(async () =>
            {
                CurrentPage = 1;
                await LoadUsersAsync();
            });
            ClearFiltersCommand = new RelayCommand(() => ClearFilters());
            PreviousPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToPreviousPage)
                {
                    CurrentPage--;
                    await LoadUsersAsync();
                }
            }, () => CanGoToPreviousPage);
            NextPageCommand = new RelayCommand(async () =>
            {
                if (CanGoToNextPage)
                {
                    CurrentPage++;
                    await LoadUsersAsync();
                }
            }, () => CanGoToNextPage);
            CloseUserDetailCommand = new RelayCommand(() => CloseUserDetail());

            // Load data khi khởi tạo
            _ = LoadUsersAsync();
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                var filter = new UserFilterRequest
                {
                    Email = string.IsNullOrWhiteSpace(SearchEmail) ? null : SearchEmail,
                    FullName = string.IsNullOrWhiteSpace(SearchFullName) ? null : SearchFullName,
                    Enabled = EnabledFilter,
                    From = FilterFromDate,
                    To = FilterToDate,
                    SortBy = SortBy,
                    SortDir = SortDir,
                    Page = CurrentPage,
                    PageSize = PageSize
                };

                var response = await _userService.GetUsersAsync(filter);

                if (response != null && response.Items != null)
                {
                    Users = new ObservableCollection<User>(response.Items);
                    TotalItems = response.TotalItems;
                    TotalPages = response.TotalPages;
                }
                else
                {
                    Users = new ObservableCollection<User>();
                    TotalItems = 0;
                    TotalPages = 0;
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách người dùng: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadUserDetailAsync(long userId)
        {
            try
            {
                IsLoading = true;
                var detail = await _userService.GetUserByIdAsync(userId);
                UserDetail = detail;
                ShowUserDetail = detail != null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải chi tiết người dùng: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ClearFilters()
        {
            SearchEmail = string.Empty;
            SearchFullName = string.Empty;
            EnabledFilter = null;
            FilterFromDate = null;
            FilterToDate = null;
            SortBy = "createdAt";
            SortDir = "desc";
            CurrentPage = 1;
            _ = LoadUsersAsync();
        }

        private void CloseUserDetail()
        {
            ShowUserDetail = false;
            UserDetail = null;
            SelectedUser = null;
        }
    }
}
