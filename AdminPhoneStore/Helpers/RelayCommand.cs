using System.Windows.Input;

namespace AdminPhoneStore.Helpers
{
    /// <summary>
    /// RelayCommand implementation với support cho CommandManager.RequerySuggested
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;

            // Subscribe vào CommandManager để tự động re-evaluate CanExecute
            if (_canExecute != null)
            {
                CommandManager.RequerySuggested += OnRequerySuggested;
            }
        }

        public bool CanExecute(object? parameter)
            => _canExecute == null || _canExecute();

        public void Execute(object? parameter)
            => _execute();

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        private void OnRequerySuggested(object? sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }
    }
}
