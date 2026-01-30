namespace AdminPhoneStore.ViewModels
{
    class DashboardViewModel : BaseViewModel
    {
        private string _title = "Dashboard Admin 123";

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
