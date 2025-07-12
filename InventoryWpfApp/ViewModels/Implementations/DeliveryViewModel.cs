using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventoryWpfApp.ViewModels.Implementations
{
    public class DeliveryViewModel : BaseViewModel
    {
        private readonly IMovementRepository _movementRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IInventoryStockRepository _inventoryStockRepository;

        private ObservableCollection<Employee> _employees;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
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

        private ObservableCollection<InventoryStock> _availableSizes;
        public ObservableCollection<InventoryStock> AvailableSizes
        {
            get => _availableSizes;
            set
            {
                _availableSizes = value;
                OnPropertyChanged();
            }
        }

        private int _selectedEmployeeId;
        public int SelectedEmployeeId
        {
            get => _selectedEmployeeId;
            set
            {
                _selectedEmployeeId = value;
                OnPropertyChanged();
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
                LoadAvailableSizesForSelectedProduct();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _selectedInventoryStockId; // This holds the InventoryStockId for the selected size
        public int SelectedInventoryStockId
        {
            get => _selectedInventoryStockId;
            set
            {
                _selectedInventoryStockId = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _quantityToDeliver;
        public string QuantityToDeliver
        {
            get => _quantityToDeliver;
            set
            {
                _quantityToDeliver = value;
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

        public ICommand RegisterDeliveryCommand { get; private set; }
        public ICommand ClearDeliveryFieldsCommand { get; private set; }

        public DeliveryViewModel(IMovementRepository movementRepository, IEmployeeRepository employeeRepository, IProductRepository productRepository, IInventoryStockRepository inventoryStockRepository)
        {
            _movementRepository = movementRepository ?? throw new ArgumentNullException(nameof(movementRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _inventoryStockRepository = inventoryStockRepository ?? throw new ArgumentNullException(nameof(inventoryStockRepository));

            LoadData();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            RegisterDeliveryCommand = new RelayCommand(RegisterDelivery, CanRegisterDelivery);
            ClearDeliveryFieldsCommand = new RelayCommand(ClearDeliveryFields);
        }

        private void LoadData()
        {
            try
            {
                Employees = new ObservableCollection<Employee>(_employeeRepository.GetAll());
                Products = new ObservableCollection<Product>(_productRepository.GetAll());
                AvailableSizes = new ObservableCollection<InventoryStock>(); // Initialize empty
            }
            catch (Exception ex)
            {
                Message = $"Error loading initial data: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void LoadAvailableSizesForSelectedProduct()
        {
            if (SelectedProductId > 0)
            {
                try
                {
                    AvailableSizes = new ObservableCollection<InventoryStock>(_inventoryStockRepository.GetAvailableSizesForProduct(SelectedProductId));
                    SelectedInventoryStockId = 0; // Reset selected size
                }
                catch (Exception ex)
                {
                    Message = $"Error loading available sizes: {ex.Message}";
                    MessageType = MessageType.Error;
                }
            }
            else
            {
                AvailableSizes.Clear();
            }
        }

        private void RegisterDelivery(object parameter)
        {
            if (!int.TryParse(QuantityToDeliver, out int quantity) || quantity <= 0)
            {
                Message = "Invalid quantity. Must be a positive number.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                _movementRepository.RegisterDelivery(SelectedInventoryStockId, SelectedEmployeeId, quantity);
                Message = "Delivery registered successfully.";
                MessageType = MessageType.Success;
                ClearDeliveryFields(null);
                // Optionally, trigger a refresh on MovementHistoryViewModel if it's visible.
            }
            catch (Exception ex)
            {
                Message = $"Error registering delivery: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanRegisterDelivery(object parameter)
        {
            return SelectedEmployeeId > 0 && SelectedInventoryStockId > 0 &&
                   int.TryParse(QuantityToDeliver, out int quantity) && quantity > 0;
        }

        private void ClearDeliveryFields(object parameter)
        {
            SelectedEmployeeId = 0;
            SelectedProductId = 0;
            SelectedInventoryStockId = 0;
            QuantityToDeliver = string.Empty;
            AvailableSizes.Clear();
            //Message = string.Empty;
            //MessageType = MessageType.None; // Reset message type
        }
    }
}
