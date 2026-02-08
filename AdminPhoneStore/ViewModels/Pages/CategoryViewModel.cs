using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class CategoryViewModel : BaseViewModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Category> _categories = new();
        private Category? _selectedCategory;
        private bool _isLoading;
        private bool _isEditMode;
        private string _searchText = string.Empty;

        // Form fields
        private string _name = string.Empty;
        private string? _description;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadCategoryForEdit(value);
                }
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

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterCategories();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoadCategoriesCommand { get; }
        public RelayCommand AddCategoryCommand { get; }
        public RelayCommand EditCategoryCommand { get; }
        public RelayCommand SaveCategoryCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand DeleteCategoryCommand { get; }

        public CategoryViewModel(
            ICategoryService categoryService,
            IDialogService dialogService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadCategoriesCommand = new RelayCommand(async () => await LoadCategoriesAsync());
            AddCategoryCommand = new RelayCommand(() => ShowAddForm());
            EditCategoryCommand = new RelayCommand(() =>
            {
                if (SelectedCategory != null)
                {
                    LoadCategoryForEdit(SelectedCategory);
                }
            }, () => SelectedCategory != null);
            SaveCategoryCommand = new RelayCommand(async () => await SaveCategoryAsync());
            CancelEditCommand = new RelayCommand(() => CancelEdit());
            DeleteCategoryCommand = new RelayCommand(async () => await DeleteCategoryAsync(), () => SelectedCategory != null);

            // Load data khi khởi tạo
            _ = LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsLoading = true;
                var categories = await _categoryService.GetAllCategoriesAsync();
                Categories = new ObservableCollection<Category>(categories);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách danh mục: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowAddForm()
        {
            SelectedCategory = null;
            Name = string.Empty;
            Description = string.Empty;
            IsEditMode = true;
        }

        private void LoadCategoryForEdit(Category category)
        {
            Name = category.Name;
            Description = category.Description;
            IsEditMode = true;
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            SelectedCategory = null;
            Name = string.Empty;
            Description = string.Empty;
        }

        private async Task SaveCategoryAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    _dialogService.ShowError("Vui lòng nhập tên danh mục", "Lỗi");
                    return;
                }

                IsLoading = true;

                if (SelectedCategory == null)
                {
                    // Create new
                    var request = new CreateCategoryRequest
                    {
                        Name = Name,
                        Description = Description
                    };

                    var newCategory = await _categoryService.CreateCategoryAsync(request);
                    if (newCategory != null)
                    {
                        _dialogService.ShowSuccess("Tạo danh mục thành công!");
                        await LoadCategoriesAsync();
                        CancelEdit();
                    }
                }
                else
                {
                    // Update existing
                    var request = new UpdateCategoryRequest
                    {
                        Name = Name,
                        Description = Description
                    };

                    var updatedCategory = await _categoryService.UpdateCategoryAsync(SelectedCategory.Id, request);
                    if (updatedCategory != null)
                    {
                        _dialogService.ShowSuccess("Cập nhật danh mục thành công!");
                        await LoadCategoriesAsync();
                        CancelEdit();
                    }
                }
            }
            catch (ApiException ex)
            {
                // Hiển thị message từ API (có thể là validation error hoặc business logic error)
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi lưu danh mục: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteCategoryAsync()
        {
            if (SelectedCategory == null) return;

            try
            {
                bool confirmed = _dialogService.ShowConfirmation(
                    $"Bạn có chắc muốn xóa danh mục '{SelectedCategory.Name}'?",
                    "Xác nhận xóa",
                    "Xóa",
                    "Hủy"
                );

                if (!confirmed) return;

                IsLoading = true;
                var success = await _categoryService.DeleteCategoryAsync(SelectedCategory.Id);
                if (success)
                {
                    _dialogService.ShowSuccess("Xóa danh mục thành công!");
                    await LoadCategoriesAsync();
                    CancelEdit();
                }
            }
            catch (ApiException ex)
            {
                // Hiển thị message từ API (ví dụ: "Cannot delete category with existing products")
                var errorMessage = ex.Message;
                if (ex.Message.Contains("API Error:"))
                {
                    errorMessage = ex.Message.Replace("API Error:", "").Trim();
                }
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa danh mục: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterCategories()
        {
            // Filter logic có thể được thêm sau nếu cần
            // Hiện tại chỉ reload toàn bộ
        }
    }
}
