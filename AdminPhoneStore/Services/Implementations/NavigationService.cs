using AdminPhoneStore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPhoneStore.Services.Implementations
{
    /// <summary>
    /// Implementation của INavigationService với View caching và navigation history
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, UserControl> _viewCache;
        private readonly Stack<UserControl> _navigationStack;
        private UserControl? _currentView;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _viewCache = new Dictionary<Type, UserControl>();
            _navigationStack = new Stack<UserControl>();
        }

        public UserControl? CurrentView
        {
            get => _currentView;
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    ViewChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanGoBack => _navigationStack.Count > 0;

        public event EventHandler? ViewChanged;

        public void NavigateTo<T>() where T : UserControl
        {
            NavigateTo<T>(null);
        }

        public void NavigateTo<T>(object? parameter) where T : UserControl
        {
            Type viewType = typeof(T);

            // Kiểm tra cache trước
            if (!_viewCache.TryGetValue(viewType, out UserControl? view))
            {
                // Resolve View từ DI container (nếu có) hoặc tạo mới
                view = _serviceProvider.GetService<T>() ?? (T)Activator.CreateInstance(viewType)!;
                _viewCache[viewType] = view;
            }

            // Thêm View hiện tại vào stack (nếu có)
            if (CurrentView != null && CurrentView != view)
            {
                _navigationStack.Push(CurrentView);
            }

            // Set View mới
            CurrentView = view;

            // Nếu View có ViewModel và ViewModel implement INavigationAware, gọi OnNavigatedTo
            if (view.DataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedTo(parameter);
            }
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;

            var previousView = _navigationStack.Pop();
            CurrentView = previousView;

            // Gọi OnNavigatedBack nếu ViewModel implement INavigationAware
            if (previousView.DataContext is INavigationAware navigationAware)
            {
                navigationAware.OnNavigatedBack();
            }
        }
    }

    /// <summary>
    /// Interface cho ViewModels muốn nhận thông báo khi navigate
    /// </summary>
    public interface INavigationAware
    {
        void OnNavigatedTo(object? parameter);
        void OnNavigatedBack();
    }
}
