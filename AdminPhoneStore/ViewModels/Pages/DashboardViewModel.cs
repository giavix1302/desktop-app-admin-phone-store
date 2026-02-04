using AdminPhoneStore.Helpers;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IDialogService _dialogService;
        private string _title = "Dashboard Admin 123";

        public RelayCommand RefreshCommand { get; }

        public DashboardViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            RefreshCommand = new RelayCommand(OnRefresh);
        }

        private void OnRefresh()
        {
            _dialogService.ShowMessage("Đã bấm Refresh", "Thông báo");
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }
}
