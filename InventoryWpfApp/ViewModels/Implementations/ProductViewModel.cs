using System.Collections.ObjectModel;
using System.Windows.Input;
using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;

namespace InventoryWpfApp.ViewModels.Implementations
{
    /// <summary>
    /// Represents the ViewModel for managing products in the inventory application.
    /// </summary>
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductRepository _productRepository;

        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
                UpdateProductFields();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _newProductName;
        public string NewProductName
        {
            get => _newProductName;
            set
            {
                _newProductName = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _newProductDescription;
        public string NewProductDescription
        {
            get => _newProductDescription;
            set
            {
                _newProductDescription = value;
                OnPropertyChanged();
            }
        }

        private string _selectedApplicabilityType;
        public string SelectedApplicabilityType
        {
            get => _selectedApplicabilityType;
            set
            {
                _selectedApplicabilityType = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private MessageType _messageType;
        public MessageType MessageType
        {
            get => _messageType;
            set
            {
                _messageType = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddProductCommand { get; private set; }
        public ICommand UpdateProductCommand { get; private set; }
        public ICommand DeleteProductCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the ProductViewModel class.
        /// </summary>
        /// <param name="productRepository">The repository for managing product data.</param>
        public ProductViewModel(IProductRepository productRepository)
        {
            _productRepository =
                productRepository ?? throw new ArgumentNullException(nameof(productRepository));

            LoadData();
            InitializeCommands();
        }

        /// <summary>
        /// Initializes the commands used in the ViewModel.
        /// </summary>
        private void InitializeCommands()
        {
            AddProductCommand = new RelayCommand(AddProduct, CanAddProduct);
            UpdateProductCommand = new RelayCommand(UpdateProduct, CanUpdateOrDeleteProduct);
            DeleteProductCommand = new RelayCommand(DeleteProduct, CanUpdateOrDeleteProduct);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
        }

        /// <summary>
        /// Loads the initial data for the ViewModel.
        /// </summary>
        private void LoadData()
        {
            try
            {
                Products = new ObservableCollection<Product>(_productRepository.GetAll());
                // Default selection for applicability
                SelectedApplicabilityType = "Administrativo";
            }
            catch (Exception ex)
            {
                Message = $"Error loading data: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        /// <summary>
        /// Adds a new product to the inventory.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private void AddProduct(object parameter)
        {
            // Validate input
            if (
                string.IsNullOrWhiteSpace(NewProductName)
                || string.IsNullOrWhiteSpace(NewProductDescription)
                || string.IsNullOrWhiteSpace(SelectedApplicabilityType)
            )
            {
                Message = "Please enter valid product name, description and applicability type.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                var newProduct = new Product
                {
                    Name = NewProductName,
                    Description = NewProductDescription,
                    ApplicabilityType = SelectedApplicabilityType,
                };
                _productRepository.Add(newProduct);
                LoadData(); // Refresh list
                NewProductName = string.Empty;
                NewProductDescription = string.Empty;
                SelectedApplicabilityType = "Administrativo";
                Message = "Product added successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error adding product: {ex.Message}";
            }
        }

        /// <summary>
        /// Checks if the AddProduct command can be executed.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private bool CanAddProduct(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewProductName)
                && !string.IsNullOrWhiteSpace(SelectedApplicabilityType);
        }

        /// <summary>
        /// Updates the selected product in the inventory.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private void UpdateProduct(object parameter)
        {
            // Validate if a product is selected and input is valid
            if (SelectedProduct is null)
            {
                Message = "Please select a product to update.";
                MessageType = MessageType.Error;
                return;
            }

            if (
                string.IsNullOrWhiteSpace(NewProductName)
                || string.IsNullOrWhiteSpace(NewProductDescription)
                || string.IsNullOrWhiteSpace(SelectedApplicabilityType)
            )
            {
                Message = "Please enter valid product name, description and applicability type.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                if (SelectedProduct != null)
                {
                    SelectedProduct.Name = NewProductName;
                    SelectedProduct.Description = NewProductDescription;
                    SelectedProduct.ApplicabilityType = SelectedApplicabilityType;
                    _productRepository.Update(SelectedProduct);
                    LoadData(); // Refresh list
                    Message = "Product updated successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error updating product: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        /// <summary>
        /// Deletes the selected product from the inventory.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private void DeleteProduct(object parameter)
        {
            try
            {
                if (SelectedProduct != null)
                {
                    _productRepository.Delete(SelectedProduct.ProductId);
                    LoadData(); // Refresh list
                    SelectedProduct = null; // Clear selection
                    Message = "Product deleted successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error deleting product: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        /// <summary>
        /// Checks if the Update or Delete commands can be executed.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private bool CanUpdateOrDeleteProduct(object parameter)
        {
            return SelectedProduct != null;
        }

        /// <summary>
        /// Clears the selection and resets the input fields.
        /// </summary>
        /// <param name="parameter">Command parameter (not used).</param>
        private void ClearSelection(object parameter)
        {
            SelectedProduct = null;
            NewProductName = string.Empty;
            NewProductDescription = string.Empty;
            SelectedApplicabilityType = "Administrativo";
            Message = string.Empty;
            MessageType = MessageType.None;
        }

        /// <summary>
        /// Updates the product fields based on the selected product.
        /// </summary>
        private void UpdateProductFields()
        {
            if (SelectedProduct != null)
            {
                NewProductName = SelectedProduct.Name;
                NewProductDescription = SelectedProduct.Description;
                SelectedApplicabilityType = SelectedProduct.ApplicabilityType;
            }
            else
            {
                NewProductName = string.Empty;
                NewProductDescription = string.Empty;
                SelectedApplicabilityType = "Administrativo";
            }
        }
    }
}
