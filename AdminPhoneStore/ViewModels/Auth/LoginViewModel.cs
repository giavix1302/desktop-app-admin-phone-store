using AdminPhoneStore.Helpers;
using AdminPhoneStore.Services.Auth;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Infrastructure;
using AdminPhoneStore.Infrastructure;
using AdminPhoneStore.ViewModels.Base;
using System.Text.RegularExpressions;
using System.Windows;
using System.Linq;

namespace AdminPhoneStore.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading = false;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
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

        public RelayCommand LoginCommand { get; }

        public LoginViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            LoginCommand = new RelayCommand(
                async () => await LoginAsync(),
                () => !IsLoading && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password)
            );

            // Subscribe to property changes to enable/disable login button
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Email) || e.PropertyName == nameof(Password) || e.PropertyName == nameof(IsLoading))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }
            };
        }

        private async Task LoginAsync()
        {
            // Validation
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                IsLoading = true;

                var success = await _authenticationService.LoginAsync(Email, Password);

                if (success)
                {
                    // Hiển thị thông báo thành công
                    _dialogService.ShowMessage("Đăng nhập thành công!", "Thông báo");

                    // Chuyển sang MainWindow
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Tìm LoginWindow và đóng nó
                        var loginWindow = Application.Current.Windows.OfType<Windows.Auth.LoginWindow>().FirstOrDefault();
                        if (loginWindow != null)
                        {
                            loginWindow.Close();
                        }

                        // Mở MainWindow
                        var mainWindow = ServiceLocator.GetService<MainWindow>();
                        mainWindow.Show();
                    });
                }
                else
                {
                    _dialogService.ShowError("Đăng nhập thất bại. Vui lòng kiểm tra lại email và mật khẩu.", "Lỗi đăng nhập");
                }
            }
            catch (ApiException ex)
            {
                _dialogService.ShowError($"Lỗi: {ex.Message}", "Lỗi đăng nhập");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Đã xảy ra lỗi: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool ValidateInput()
        {
            // Validate Email
            if (string.IsNullOrWhiteSpace(Email))
            {
                _dialogService.ShowError("Vui lòng nhập email.", "Lỗi validation");
                return false;
            }

            // Validate Email Format
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(Email))
            {
                _dialogService.ShowError("Email không đúng định dạng.", "Lỗi validation");
                return false;
            }

            // Validate Password
            if (string.IsNullOrWhiteSpace(Password))
            {
                _dialogService.ShowError("Vui lòng nhập mật khẩu.", "Lỗi validation");
                return false;
            }

            if (Password.Length < 6)
            {
                _dialogService.ShowError("Mật khẩu phải có ít nhất 6 ký tự.", "Lỗi validation");
                return false;
            }

            return true;
        }
    }
}
