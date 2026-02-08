using AdminPhoneStore.Helpers;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.ViewModels.Base;
using AdminPhoneStore.Infrastructure;
using System.Windows;
using System.Windows.Controls;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;
        private UserControl? _currentView;
        private string _activeMenu = "Dashboard";

        public UserControl? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public string ActiveMenu
        {
            get => _activeMenu;
            set
            {
                _activeMenu = value;
                OnPropertyChanged();
            }
        }

        public string UserName => _authenticationService.CurrentUser?.FullName ?? "Admin User";
        public string UserEmail => _authenticationService.CurrentUser?.Email ?? "admin@example.com";

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowProductCommand { get; }
        public RelayCommand ShowCategoryCommand { get; }
        public RelayCommand ShowBrandCommand { get; }
        public RelayCommand ShowOrderCommand { get; }
        public RelayCommand ShowCustomerCommand { get; }
        public RelayCommand GoBackCommand { get; }
        public RelayCommand LogoutCommand { get; }

        public ShellViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IDialogService dialogService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            // Subscribe vào event khi View thay đổi
            _navigationService.ViewChanged += OnViewChanged;

            // Navigate đến Dashboard mặc định
            _navigationService.NavigateTo<Views.Pages.DashboardView>();

            // Setup Commands
            ShowDashboardCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Dashboard";
                _navigationService.NavigateTo<Views.Pages.DashboardView>();
            });

            ShowProductCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Product";
                _navigationService.NavigateTo<Views.Pages.ProductView>();
            });

            ShowCategoryCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Category";
                _navigationService.NavigateTo<Views.Pages.CategoryView>();
            });

            ShowBrandCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Brand";
                _navigationService.NavigateTo<Views.Pages.BrandView>();
            });

            ShowOrderCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Orders";
                _navigationService.NavigateTo<Views.Pages.OrderView>();
            });

            ShowCustomerCommand = new RelayCommand(() =>
            {
                ActiveMenu = "Customers";
                _navigationService.NavigateTo<Views.Pages.UserView>();
            });

            GoBackCommand = new RelayCommand(
                () => _navigationService.GoBack(),
                () => _navigationService.CanGoBack
            );

            LogoutCommand = new RelayCommand(() => { _ = LogoutAsync(); });
        }

        private async Task LogoutAsync()
        {
            try
            {
                // Hiển thị xác nhận
                bool confirmed = _dialogService.ShowConfirmation(
                    "Bạn có chắc muốn đăng xuất?",
                    "Xác nhận đăng xuất",
                    "Đăng xuất",
                    "Hủy"
                );

                if (!confirmed)
                {
                    return;
                }

                // Gọi logout API
                await _authenticationService.LogoutAsync();

                // Hiển thị thông báo thành công
                _dialogService.ShowSuccess("Đăng xuất thành công!");

                // Đóng MainWindow và hiển thị LoginWindow
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Đóng MainWindow hiện tại
                    var mainWindow = Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.Close();
                    }

                    // Hiển thị LoginWindow
                    var loginWindow = ServiceLocator.GetService<Windows.Auth.LoginWindow>();
                    Application.Current.MainWindow = loginWindow;
                    loginWindow.Show();
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi");
            }
        }

        private void OnViewChanged(object? sender, EventArgs e)
        {
            CurrentView = _navigationService.CurrentView;
        }
    }
}
