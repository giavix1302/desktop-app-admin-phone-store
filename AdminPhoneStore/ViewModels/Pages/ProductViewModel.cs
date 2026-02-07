using AdminPhoneStore.Helpers;
using AdminPhoneStore.Models;
using AdminPhoneStore.Services.Business;
using AdminPhoneStore.Services.UI;
using AdminPhoneStore.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;

namespace AdminPhoneStore.ViewModels.Pages
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IColorService _colorService;
        private readonly IProductImageService _productImageService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<Product> _products = new();
        private Product? _selectedProduct;
        private bool _isLoading;
        private bool _isEditMode;
        private string _searchText = string.Empty;

        // Form fields
        private string _name = string.Empty;
        private string? _description;
        private decimal _price;
        private decimal? _discountPrice;
        private int _stockQuantity;
        private bool _isActive = true;
        private long? _selectedCategoryId;
        private long? _selectedBrandId;
        private ObservableCollection<long> _selectedColorIds = new();
        private ObservableCollection<SpecificationRequest> _specifications = new();

        // Dropdowns
        private ObservableCollection<Category> _categories = new();
        private ObservableCollection<Brand> _brands = new();
        private ObservableCollection<Color> _colors = new();

        // Product Images
        private ObservableCollection<ProductImage> _productImages = new();

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadProductForEdit(value);
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
            }
        }

        // Form properties
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

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public decimal? DiscountPrice
        {
            get => _discountPrice;
            set
            {
                _discountPrice = value;
                OnPropertyChanged();
            }
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set
            {
                _stockQuantity = value;
                OnPropertyChanged();
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }

        public long? SelectedCategoryId
        {
            get => _selectedCategoryId;
            set
            {
                _selectedCategoryId = value;
                OnPropertyChanged();
            }
        }

        public long? SelectedBrandId
        {
            get => _selectedBrandId;
            set
            {
                _selectedBrandId = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<long> SelectedColorIds
        {
            get => _selectedColorIds;
            set
            {
                _selectedColorIds = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SpecificationRequest> Specifications
        {
            get => _specifications;
            set
            {
                _specifications = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Brand> Brands
        {
            get => _brands;
            set
            {
                _brands = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Color> Colors
        {
            get => _colors;
            set
            {
                _colors = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand LoadProductsCommand { get; }
        public RelayCommand LoadDropdownsCommand { get; }
        public RelayCommand AddProductCommand { get; }
        public RelayCommand EditProductCommand { get; }
        public RelayCommand SaveProductCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand DeleteProductCommand { get; }
        public RelayCommand AddSpecificationCommand { get; }
        public RelayCommand<SpecificationRequest> RemoveSpecificationCommand { get; }
        public RelayCommand LoadProductImagesCommand { get; }
        public RelayCommand UploadImageCommand { get; }
        public RelayCommand<ProductImage> DeleteImageCommand { get; }
        public RelayCommand<ProductImage> SetPrimaryImageCommand { get; }

        // Helper property để bind với ListBox SelectedItems
        private ObservableCollection<Color> _selectedColors = new();
        public ObservableCollection<Color> SelectedColors
        {
            get => _selectedColors;
            set
            {
                _selectedColors = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductImage> ProductImages
        {
            get => _productImages;
            set
            {
                _productImages = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel(
            IProductService productService,
            ICategoryService categoryService,
            IBrandService brandService,
            IColorService colorService,
            IProductImageService productImageService,
            IDialogService dialogService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _colorService = colorService ?? throw new ArgumentNullException(nameof(colorService));
            _productImageService = productImageService ?? throw new ArgumentNullException(nameof(productImageService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));

            LoadProductsCommand = new RelayCommand(async () => await LoadProductsAsync());
            LoadDropdownsCommand = new RelayCommand(async () => await LoadDropdownsAsync());
            AddProductCommand = new RelayCommand(() => ShowAddForm());
            EditProductCommand = new RelayCommand(() =>
            {
                if (SelectedProduct != null)
                {
                    LoadProductForEdit(SelectedProduct);
                }
            }, () => SelectedProduct != null);
            SaveProductCommand = new RelayCommand(async () => await SaveProductAsync());
            CancelEditCommand = new RelayCommand(() => CancelEdit());
            DeleteProductCommand = new RelayCommand(async () => await DeleteProductAsync(), () => SelectedProduct != null);
            AddSpecificationCommand = new RelayCommand(() => AddSpecification());
            RemoveSpecificationCommand = new RelayCommand<SpecificationRequest>((spec) => RemoveSpecification(spec));
            LoadProductImagesCommand = new RelayCommand(async () => await LoadProductImagesAsync());
            UploadImageCommand = new RelayCommand(async () => await UploadImageAsync());
            DeleteImageCommand = new RelayCommand<ProductImage>(async (image) => await DeleteImageAsync(image));
            SetPrimaryImageCommand = new RelayCommand<ProductImage>(async (image) => await SetPrimaryImageAsync(image));

            // Load data khi khởi tạo
            _ = LoadDropdownsAsync();
            _ = LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                IsLoading = true;
                var products = await _productService.GetAllProductsAsync();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Products.Clear();
                    foreach (var product in products)
                    {
                        Products.Add(product);
                    }
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải danh sách sản phẩm: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadDropdownsAsync()
        {
            try
            {
                var categoriesTask = _categoryService.GetAllCategoriesAsync();
                var brandsTask = _brandService.GetAllBrandsAsync();
                var colorsTask = _colorService.GetAllColorsAsync();

                await Task.WhenAll(categoriesTask, brandsTask, colorsTask);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Categories.Clear();
                    foreach (var category in categoriesTask.Result)
                    {
                        Categories.Add(category);
                    }

                    Brands.Clear();
                    foreach (var brand in brandsTask.Result)
                    {
                        Brands.Add(brand);
                    }

                    Colors.Clear();
                    foreach (var color in colorsTask.Result)
                    {
                        Colors.Add(color);
                    }

                    // Nếu đang edit và có SelectedColorIds, sync lại SelectedColors
                    if (SelectedColorIds.Count > 0 && Colors.Count > 0)
                    {
                        SelectedColors.Clear();
                        foreach (var colorId in SelectedColorIds)
                        {
                            var colorObj = Colors.FirstOrDefault(c => c.Id == colorId);
                            if (colorObj != null)
                            {
                                SelectedColors.Add(colorObj);
                            }
                        }
                        OnPropertyChanged(nameof(SelectedColors));
                    }
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải dữ liệu dropdown: {ex.Message}", "Lỗi");
            }
        }

        private void ShowAddForm()
        {
            IsEditMode = true;
            SelectedProduct = null;
            ResetForm();
        }

        private void LoadProductForEdit(Product product)
        {
            Name = product.Name;
            Description = product.Description;
            Price = product.Price;
            DiscountPrice = product.DiscountPrice;
            StockQuantity = product.StockQuantity;
            IsActive = product.IsActive;
            SelectedCategoryId = product.CategoryId;
            SelectedBrandId = product.BrandId;
            SelectedColorIds.Clear();
            SelectedColors.Clear();

            // Đảm bảo Colors đã được load trước khi sync
            if (Colors.Count > 0)
            {
                foreach (var color in product.Colors)
                {
                    SelectedColorIds.Add(color.Id);
                    var colorObj = Colors.FirstOrDefault(c => c.Id == color.Id);
                    if (colorObj != null)
                    {
                        SelectedColors.Add(colorObj);
                    }
                }
            }
            else
            {
                // Nếu Colors chưa load, lưu lại để sync sau
                foreach (var color in product.Colors)
                {
                    SelectedColorIds.Add(color.Id);
                }
            }

            Specifications.Clear();
            foreach (var spec in product.Specifications)
            {
                Specifications.Add(new SpecificationRequest
                {
                    SpecName = spec.SpecName,
                    SpecValue = spec.SpecValue
                });
            }
            IsEditMode = true;

            // Trigger PropertyChanged để sync với UI
            OnPropertyChanged(nameof(SelectedColors));

            // Load product images
            if (product.Id > 0)
            {
                _ = LoadProductImagesAsync();
            }
        }

        private void ResetForm()
        {
            Name = string.Empty;
            Description = null;
            Price = 0;
            DiscountPrice = null;
            StockQuantity = 0;
            IsActive = true;
            SelectedCategoryId = null;
            SelectedBrandId = null;
            SelectedColorIds.Clear();
            SelectedColors.Clear();
            Specifications.Clear();
            ProductImages.Clear();
        }

        private void CancelEdit()
        {
            IsEditMode = false;
            SelectedProduct = null;
            ResetForm();
        }

        private async Task SaveProductAsync()
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(Name))
                {
                    _dialogService.ShowError("Vui lòng nhập tên sản phẩm", "Lỗi");
                    return;
                }

                if (Price <= 0)
                {
                    _dialogService.ShowError("Giá sản phẩm phải lớn hơn 0", "Lỗi");
                    return;
                }

                if (StockQuantity < 0)
                {
                    _dialogService.ShowError("Số lượng tồn kho không được âm", "Lỗi");
                    return;
                }

                if (!SelectedCategoryId.HasValue)
                {
                    _dialogService.ShowError("Vui lòng chọn danh mục", "Lỗi");
                    return;
                }

                if (!SelectedBrandId.HasValue)
                {
                    _dialogService.ShowError("Vui lòng chọn thương hiệu", "Lỗi");
                    return;
                }

                if (SelectedColors.Count == 0)
                {
                    _dialogService.ShowError("Vui lòng chọn ít nhất 1 màu", "Lỗi");
                    return;
                }

                // Sync SelectedColors to SelectedColorIds
                SelectedColorIds.Clear();
                foreach (var color in SelectedColors)
                {
                    SelectedColorIds.Add(color.Id);
                }

                IsLoading = true;

                if (SelectedProduct == null)
                {
                    // Create
                    var request = new CreateProductRequest
                    {
                        Name = Name,
                        Description = Description,
                        Price = Price,
                        DiscountPrice = DiscountPrice,
                        StockQuantity = StockQuantity,
                        CategoryId = SelectedCategoryId.Value,
                        BrandId = SelectedBrandId.Value,
                        ColorIds = SelectedColorIds.ToList(),
                        Specifications = Specifications.Count > 0 ? Specifications.ToList() : null
                    };

                    await _productService.CreateProductAsync(request);
                    _dialogService.ShowSuccess("Tạo sản phẩm thành công!");
                }
                else
                {
                    // Update
                    var request = new UpdateProductRequest
                    {
                        Name = Name,
                        Description = Description,
                        Price = Price,
                        DiscountPrice = DiscountPrice,
                        StockQuantity = StockQuantity,
                        IsActive = IsActive,
                        CategoryId = SelectedCategoryId.Value,
                        BrandId = SelectedBrandId.Value,
                        ColorIds = SelectedColorIds.ToList(),
                        Specifications = Specifications.Count > 0 ? Specifications.ToList() : null
                    };

                    await _productService.UpdateProductAsync(SelectedProduct.Id, request);
                    _dialogService.ShowSuccess("Cập nhật sản phẩm thành công!");
                }

                await LoadProductsAsync();
                CancelEdit();
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi lưu sản phẩm: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            bool confirmed = _dialogService.ShowConfirmation(
                $"Bạn có chắc muốn xóa sản phẩm '{SelectedProduct.Name}'?",
                "Xác nhận xóa",
                "Xóa",
                "Hủy"
            );

            if (!confirmed) return;

            try
            {
                IsLoading = true;
                await _productService.DeleteProductAsync(SelectedProduct.Id);
                _dialogService.ShowSuccess("Xóa sản phẩm thành công!");
                await LoadProductsAsync();
                SelectedProduct = null;
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa sản phẩm: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AddSpecification()
        {
            Specifications.Add(new SpecificationRequest());
        }

        private void RemoveSpecification(SpecificationRequest? spec)
        {
            if (spec != null)
            {
                Specifications.Remove(spec);
            }
        }

        private async Task LoadProductImagesAsync()
        {
            if (SelectedProduct == null || SelectedProduct.Id <= 0) return;

            try
            {
                IsLoading = true;
                var images = await _productImageService.GetProductImagesAsync(SelectedProduct.Id);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProductImages.Clear();
                    foreach (var image in images)
                    {
                        ProductImages.Add(image);
                    }
                });
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi tải hình ảnh sản phẩm: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UploadImageAsync()
        {
            if (SelectedProduct == null || SelectedProduct.Id <= 0)
            {
                _dialogService.ShowError("Vui lòng lưu sản phẩm trước khi upload hình ảnh", "Lỗi");
                return;
            }

            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Image files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All files (*.*)|*.*",
                    Title = "Chọn hình ảnh sản phẩm"
                };

                if (dialog.ShowDialog() == true)
                {
                    IsLoading = true;
                    var image = await _productImageService.UploadProductImageAsync(
                        SelectedProduct.Id,
                        dialog.FileName,
                        null,
                        ProductImages.Count == 0 // Auto set primary nếu là ảnh đầu tiên
                    );

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ProductImages.Add(image);
                        // Sort: primary first
                        var sorted = ProductImages.OrderByDescending(i => i.IsPrimary).ToList();
                        ProductImages.Clear();
                        foreach (var img in sorted)
                        {
                            ProductImages.Add(img);
                        }
                    });

                    _dialogService.ShowSuccess("Upload hình ảnh thành công!");
                }
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi upload hình ảnh: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteImageAsync(ProductImage? image)
        {
            if (image == null || SelectedProduct == null || SelectedProduct.Id <= 0) return;

            bool confirmed = _dialogService.ShowConfirmation(
                "Bạn có chắc muốn xóa hình ảnh này?",
                "Xác nhận xóa",
                "Xóa",
                "Hủy"
            );

            if (!confirmed) return;

            try
            {
                IsLoading = true;
                await _productImageService.DeleteProductImageAsync(SelectedProduct.Id, image.Id);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ProductImages.Remove(image);
                });

                _dialogService.ShowSuccess("Xóa hình ảnh thành công!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi xóa hình ảnh: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SetPrimaryImageAsync(ProductImage? image)
        {
            if (image == null || SelectedProduct == null || SelectedProduct.Id <= 0) return;

            if (image.IsPrimary)
            {
                _dialogService.ShowError("Hình ảnh này đã là ảnh chính", "Thông báo");
                return;
            }

            try
            {
                IsLoading = true;
                var updatedImage = await _productImageService.UpdateProductImageAsync(
                    SelectedProduct.Id,
                    image.Id,
                    null,
                    null,
                    true // Set primary
                );

                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update image in collection
                    var existingImage = ProductImages.FirstOrDefault(i => i.Id == image.Id);
                    if (existingImage != null)
                    {
                        existingImage.IsPrimary = true;
                    }

                    // Unset other primary images
                    foreach (var img in ProductImages.Where(i => i.Id != image.Id))
                    {
                        img.IsPrimary = false;
                    }

                    // Sort: primary first
                    var sorted = ProductImages.OrderByDescending(i => i.IsPrimary).ToList();
                    ProductImages.Clear();
                    foreach (var img in sorted)
                    {
                        ProductImages.Add(img);
                    }
                });

                _dialogService.ShowSuccess("Đặt ảnh chính thành công!");
            }
            catch (Exception ex)
            {
                _dialogService.ShowError($"Lỗi khi đặt ảnh chính: {ex.Message}", "Lỗi");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
