using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InventoryWpfApp.ViewModels.Implementations
{
    public class InventoryStockViewModel : BaseViewModel
    {
        private readonly IInventoryStockRepository _inventoryStockRepository;
        private readonly IProductRepository _productRepository;
        private readonly ISizeRepository _sizeRepository;

        private ObservableCollection<InventoryStock> _stockItems;
        public ObservableCollection<InventoryStock> StockItems
        {
            get => _stockItems;
            set
            {
                _stockItems = value;
                OnPropertyChanged();
            }
        }

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

        private ObservableCollection<Size> _sizes;
        public ObservableCollection<Size> Sizes
        {
            get => _sizes;
            set
            {
                _sizes = value;
                OnPropertyChanged();
            }
        }

        private InventoryStock _selectedStockItem;
        public InventoryStock SelectedStockItem
        {
            get => _selectedStockItem;
            set
            {
                _selectedStockItem = value;
                OnPropertyChanged();
                UpdateStockFields();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _selectedProductId;
        public int SelectedProductId
        {
            get => _selectedProductId;
            set
            {
                _selectedProductId = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _selectedSizeId;
        public int SelectedSizeId
        {
            get => _selectedSizeId;
            set
            {
                _selectedSizeId = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _quantityInput; // Use string for validation flexibility
        public string QuantityInput
        {
            get => _quantityInput;
            set
            {
                _quantityInput = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _minStockLimitInput; // Use string for validation flexibility
        public string MinStockLimitInput
        {
            get => _minStockLimitInput;
            set
            {
                _minStockLimitInput = value;
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

        public ICommand AddOrUpdateStockCommand { get; private set; }
        public ICommand UpdateStockCommand { get; private set; } // For modifying only current quantity/min limit of existing record
        public ICommand DeleteStockCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }

        public InventoryStockViewModel(IInventoryStockRepository inventoryStockRepository, IProductRepository productRepository, ISizeRepository sizeRepository)
        {
            _inventoryStockRepository = inventoryStockRepository ?? throw new ArgumentNullException(nameof(inventoryStockRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _sizeRepository = sizeRepository ?? throw new ArgumentNullException(nameof(sizeRepository));

            LoadData();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddOrUpdateStockCommand = new RelayCommand(AddOrUpdateStock, CanAddOrUpdateStock);
            UpdateStockCommand = new RelayCommand(UpdateExistingStock, CanUpdateOrDeleteStock);
            DeleteStockCommand = new RelayCommand(DeleteStock, CanUpdateOrDeleteStock);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
        }

        private void LoadData()
        {
            try
            {
                StockItems = new ObservableCollection<InventoryStock>(_inventoryStockRepository.GetStockDetails());
                Products = new ObservableCollection<Product>(_productRepository.GetAll());
                Sizes = new ObservableCollection<Size>(_sizeRepository.GetAll());
            }
            catch (Exception ex)
            {
                Message = $"Error loading data: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void AddOrUpdateStock(object parameter)
        {
            if (!int.TryParse(QuantityInput, out int quantity) || quantity <= 0)
            {
                Message = "Invalid quantity. Must be a positive number.";
                MessageType = MessageType.Error;
                return;
            }
            if (!int.TryParse(MinStockLimitInput, out int minStockLimit) || minStockLimit < 0)
            {
                Message = "Invalid min stock limit. Must be a non-negative number.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                _inventoryStockRepository.AddOrUpdateStock(SelectedProductId, SelectedSizeId, quantity, minStockLimit);
                LoadData();
                ClearSelection(null); // Clear fields and message
                Message = "Stock added/updated successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error adding/updating stock: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanAddOrUpdateStock(object parameter)
        {
            return SelectedProductId > 0 && SelectedSizeId > 0 &&
                   int.TryParse(QuantityInput, out int quantity) && quantity > 0 &&
                   int.TryParse(MinStockLimitInput, out int minLimit) && minLimit >= 0;
        }

        private void UpdateExistingStock(object parameter)
        {
            if (SelectedStockItem is null)
            {
                Message = "No stock item selected for update.";
                MessageType = MessageType.Error;
                return;
            }
            if (!int.TryParse(QuantityInput, out int currentQuantity) || currentQuantity < 0)
            {
                Message = "Invalid current quantity. Must be a non-negative number.";
                MessageType = MessageType.Error;
                return;
            }
            if (!int.TryParse(MinStockLimitInput, out int minStockLimit) || minStockLimit < 0)
            {
                Message = "Invalid min stock limit. Must be a non-negative number.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                SelectedStockItem.CurrentQuantity = currentQuantity;
                SelectedStockItem.MinStockLimit = minStockLimit;
                _inventoryStockRepository.Update(SelectedStockItem);
                LoadData();
                ClearSelection(null);
                Message = "Stock item updated successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error updating stock item: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanUpdateOrDeleteStock(object parameter)
        {
            return SelectedStockItem != null;
        }

        private void DeleteStock(object parameter)
        {
            try
            {
                if (SelectedStockItem != null)
                {
                    _inventoryStockRepository.Delete(SelectedStockItem.InventoryStockId);
                    LoadData(); // Refresh list
                    ClearSelection(null);
                    Message = "Stock item deleted successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error deleting stock item: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void ClearSelection(object parameter)
        {
            SelectedStockItem = null;
            SelectedProductId = 0;
            SelectedSizeId = 0;
            QuantityInput = string.Empty;
            MinStockLimitInput = string.Empty;
            Message = string.Empty;
            MessageType = MessageType.None; // Reset message type
        }

        private void UpdateStockFields()
        {
            if (SelectedStockItem != null)
            {
                SelectedProductId = SelectedStockItem.ProductId;
                SelectedSizeId = SelectedStockItem.SizeId;
                QuantityInput = SelectedStockItem.CurrentQuantity.ToString();
                MinStockLimitInput = SelectedStockItem.MinStockLimit.ToString();
            }
            else
            {
                SelectedProductId = 0;
                SelectedSizeId = 0;
                QuantityInput = string.Empty;
                MinStockLimitInput = string.Empty;
            }
        }
    }
}
