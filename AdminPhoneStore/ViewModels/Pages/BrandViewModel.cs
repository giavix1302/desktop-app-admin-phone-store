using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Api;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class BrandViewModel : BaseViewModel
    {
        private readonly IBrandService _brandService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Brand> _brands = new();
        private Brand? _selectedBrand;
        private bool _isLoading;
        private bool _isEditMode;
        private string _searchText = string.Empty;

        // Form fields
        private string _name = string.Empty;
        private string? _description;
        private string? _imageUrl;

        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set
            {
                _brands = value;
                OnPropertyChanged();
            }
        }

        public Brand? SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                _selectedBrand = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadBrandForEdit(value);
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
                FilterBrands();
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

        public string? ImageUrl
        {
            get => _imageUrl;
            set
            {
                _imageUrl = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasImage));
            }
        }

        public bool HasImage => !string.IsNullOrEmpty(ImageUrl);

        public RelayCommand LoadBrandsCommand { get; }
        public RelayCommand AddBrandCommand { get; }
        public RelayCommand EditBrandCommand { get; }
        public RelayCommand SaveBrandCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand DeleteBrandCommand { get; }
        public RelayCommand UploadImageCommand { get; }
        public RelayCommand DeleteImageCommand { get; }

        public BrandViewModel(
            IBrandService brandService,
            IDialogService dialogService)
        {
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadBrandsCommand = new RelayCommand(async () => await LoadBrandsAsync());
            AddBrandCommand = new RelayCommand(() => ShowAddForm());
            EditBrandCommand = new RelayCommand(() =>
            {
                if (SelectedBrand != null)
                {
                    LoadBrandForEdit(SelectedBrand);
                }
            }, () => SelectedBrand != null);
            SaveBrandCommand = new RelayCommand(async () => await SaveBrandAsync());
            CancelEditCommand = new RelayCommand(() => CancelEdit());
            DeleteBrandCommand = new RelayCommand(async () => await DeleteBrandAsync(), () => SelectedBrand != null);
            UploadImageCommand = new RelayCommand(async () => await UploadImageAsync(), () => SelectedBrand != null);
            DeleteImageCommand = new RelayCommand(async () => await DeleteImageAsync(), () => SelectedBrand != null && HasImage);

            // Load data khi khởi tạo
            _ = LoadBrandsAsync();
        }

        private async Task LoadBrandsAsync()
        {
            try
            {
                IsLoading = true;
                var brands = await _brandService.GetAllBrandsAsync();
                Brands = new ObservableCollection<Brand>(brands);
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách thương hiệu: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ShowAddForm()
        {
            SelectedBrand = null;
            Name = string.Empty;
            Description = string.Empty;
            ImageUrl = null;
            IsEditMode = true;
        }

        private void LoadBrandForEdit(Brand brand)
        {
            Name = brand.Name;
            Description = brand.Description;
            ImageUrl = brand.ImageUrl;
            IsEditMode = true;
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            SelectedBrand = null;
            Name = string.Empty;
            Description = string.Empty;
            ImageUrl = null;
        }

        private async Task SaveBrandAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    _dialogService.ShowError("Vui lòng nhập tên thương hiệu", "Lỗi");
                    return;
                }

                IsLoading = true;

                if (SelectedBrand == null)
                {
                    var request = new CreateBrandRequest { Name = Name, Description = Description };
                    var newBrand = await _brandService.CreateBrandAsync(request);
                    if (newBrand != null)
                    {
                        _dialogService.ShowSuccess("Tạo thương hiệu thành công!");
                        await LoadBrandsAsync();
                        CancelEdit();
                    }
                }
                else
                {
                    var request = new UpdateBrandRequest { Name = Name, Description = Description };
                    var updatedBrand = await _brandService.UpdateBrandAsync(SelectedBrand.Id, request);
                    if (updatedBrand != null)
                    {
                        _dialogService.ShowSuccess("Cập nhật thương hiệu thành công!");
                        await LoadBrandsAsync();
                        CancelEdit();
                    }
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message.Contains("API Error:") ? ex.Message.Replace("API Error:", "").Trim() : ex.Message;
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi lưu thương hiệu: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteBrandAsync()
        {
            if (SelectedBrand == null) return;

            try
            {
                bool confirmed = _dialogService.ShowConfirmation(
                    $"Bạn có chắc muốn xóa thương hiệu '{SelectedBrand.Name}'?",
                    "Xác nhận xóa", "Xóa", "Hủy");

                if (!confirmed) return;

                IsLoading = true;
                var success = await _brandService.DeleteBrandAsync(SelectedBrand.Id);
                if (success)
                {
                    _dialogService.ShowSuccess("Xóa thương hiệu thành công!");
                    await LoadBrandsAsync();
                    CancelEdit();
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message.Contains("API Error:") ? ex.Message.Replace("API Error:", "").Trim() : ex.Message;
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa thương hiệu: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UploadImageAsync()
        {
            if (SelectedBrand == null) return;

            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh thương hiệu",
                Filter = "Image files (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp"
            };

            if (dialog.ShowDialog() != true) return;

            try
            {
                IsLoading = true;
                var updated = await _brandService.UploadImageAsync(SelectedBrand.Id, dialog.FileName);
                if (updated != null)
                {
                    ImageUrl = updated.ImageUrl;
                    SelectedBrand.ImageUrl = updated.ImageUrl;
                    _dialogService.ShowSuccess("Cập nhật ảnh thành công!");
                    await LoadBrandsAsync();
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message.Contains("API Error:") ? ex.Message.Replace("API Error:", "").Trim() : ex.Message;
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi upload ảnh: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteImageAsync()
        {
            if (SelectedBrand == null) return;

            bool confirmed = _dialogService.ShowConfirmation(
                "Bạn có chắc muốn xóa ảnh thương hiệu này?",
                "Xác nhận xóa ảnh", "Xóa", "Hủy");

            if (!confirmed) return;

            try
            {
                IsLoading = true;
                var success = await _brandService.DeleteImageAsync(SelectedBrand.Id);
                if (success)
                {
                    ImageUrl = null;
                    SelectedBrand.ImageUrl = null;
                    _dialogService.ShowSuccess("Xóa ảnh thành công!");
                    await LoadBrandsAsync();
                }
            }
            catch (ApiException ex)
            {
                var errorMessage = ex.Message.Contains("API Error:") ? ex.Message.Replace("API Error:", "").Trim() : ex.Message;
                _dialogService.ShowError(errorMessage, "Lỗi");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa ảnh: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterBrands()
        {
            // Filter logic có thể được thêm sau nếu cần
        }
    }
}
