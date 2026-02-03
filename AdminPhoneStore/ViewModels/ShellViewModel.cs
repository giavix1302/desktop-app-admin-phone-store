using AdminPhoneStore.Helpers;
using AdminPhoneStore.Services.Interfaces;
using System.Windows.Controls;

namespace AdminPhoneStore.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private UserControl? _currentView;

        public UserControl? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ShowDashboardCommand { get; }
        public RelayCommand ShowProductCommand { get; }
        public RelayCommand GoBackCommand { get; }

        public ShellViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            // Subscribe vào event khi View thay đổi
            _navigationService.ViewChanged += OnViewChanged;

            // Navigate đến Dashboard mặc định
            _navigationService.NavigateTo<Views.DashboardView>();

            // Setup Commands
            ShowDashboardCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo<Views.DashboardView>();
            });

            ShowProductCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo<Views.ProductView>();
            });

            GoBackCommand = new RelayCommand(
                () => _navigationService.GoBack(),
                () => _navigationService.CanGoBack
            );
        }

        private void OnViewChanged(object? sender, EventArgs e)
        {
            CurrentView = _navigationService.CurrentView;
        }
    }
}
