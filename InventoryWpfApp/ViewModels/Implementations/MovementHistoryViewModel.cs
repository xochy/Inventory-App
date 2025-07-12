using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventoryWpfApp.ViewModels.Implementations
{
    public class MovementHistoryViewModel : BaseViewModel
    {
        private readonly IMovementRepository _movementRepository;

        private ObservableCollection<Movement> _movements;
        public ObservableCollection<Movement> Movements
        {
            get => _movements;
            set
            {
                _movements = value;
                OnPropertyChanged();
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

        public ICommand RefreshMovementsCommand { get; private set; }

        public MovementHistoryViewModel(IMovementRepository movementRepository)
        {
            _movementRepository = movementRepository ?? throw new ArgumentNullException(nameof(movementRepository));

            InitializeCommands();
            LoadData(); // Load data initially
        }

        private void InitializeCommands()
        {
            RefreshMovementsCommand = new RelayCommand(RefreshMovements);
        }

        private void RefreshMovements(object parameter = null)
        {
            try
            {
                Movements = new ObservableCollection<Movement>(_movementRepository.GetAllMovements());
                Message = "Movement history refreshed.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error loading movement history: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void LoadData()
        {
            RefreshMovements(); // Initial load
        }
    }
}
