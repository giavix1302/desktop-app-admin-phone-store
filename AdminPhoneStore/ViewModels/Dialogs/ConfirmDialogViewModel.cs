using AdminPhoneStore.Helpers;
using AdminPhoneStore.ViewModels.Base;
using System;

namespace AdminPhoneStore.ViewModels.Dialogs
{
    public class ConfirmDialogViewModel : BaseViewModel
    {
        private string _title = "Xác nhận";
        private string _message = string.Empty;
        private string _confirmText = "Xác nhận";
        private string _cancelText = "Hủy";

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmText
        {
            get => _confirmText;
            set
            {
                _confirmText = value;
                OnPropertyChanged();
            }
        }

        public string CancelText
        {
            get => _cancelText;
            set
            {
                _cancelText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event EventHandler<bool>? Result;

        public ConfirmDialogViewModel(string message, string title = "Xác nhận", string confirmText = "Xác nhận", string cancelText = "Hủy")
        {
            Message = message;
            Title = title;
            ConfirmText = confirmText;
            CancelText = cancelText;

            ConfirmCommand = new RelayCommand(() => OnResult(true));
            CancelCommand = new RelayCommand(() => OnResult(false));
        }

        private void OnResult(bool confirmed)
        {
            Result?.Invoke(this, confirmed);
        }
    }
}
