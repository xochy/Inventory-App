using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventoryWpfApp.ViewModels.Implementations
{
    public class SizeViewModel : BaseViewModel
    {
        private readonly ISizeRepository _sizeRepository;

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

        private Size _selectedSize;
        public Size SelectedSize
        {
            get => _selectedSize;
            set
            {
                _selectedSize = value;
                OnPropertyChanged();
                UpdateSizeFields();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _newSizeValue;
        public string NewSizeValue
        {
            get => _newSizeValue;
            set
            {
                _newSizeValue = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _selectedNotationType;
        public string SelectedNotationType
        {
            get => _selectedNotationType;
            set
            {
                _selectedNotationType = value;
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

        public ICommand AddSizeCommand { get; private set; }
        public ICommand UpdateSizeCommand { get; private set; }
        public ICommand DeleteSizeCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }

        public SizeViewModel(ISizeRepository sizeRepository)
        {
            _sizeRepository = sizeRepository ?? throw new ArgumentNullException(nameof(sizeRepository));

            LoadData();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddSizeCommand = new RelayCommand(AddSize, CanAddSize);
            UpdateSizeCommand = new RelayCommand(UpdateSize, CanUpdateOrDeleteSize);
            DeleteSizeCommand = new RelayCommand(DeleteSize, CanUpdateOrDeleteSize);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
        }

        private void LoadData()
        {
            try
            {
                Sizes = new ObservableCollection<Size>(_sizeRepository.GetAll());
                SelectedNotationType = "Americana"; // Default
            }
            catch (Exception ex)
            {
                Message = $"Error loading data: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void AddSize(object parameter)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(NewSizeValue) || string.IsNullOrWhiteSpace(SelectedNotationType))
            {
                Message = "Please enter valid size value and notation type.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                var newSize = new Size
                {
                    SizeValue = NewSizeValue,
                    NotationType = SelectedNotationType
                };
                _sizeRepository.Add(newSize);
                LoadData(); // Refresh list
                NewSizeValue = string.Empty;
                SelectedNotationType = "Americana";
                Message = "Size added successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error adding size: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanAddSize(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewSizeValue) && !string.IsNullOrWhiteSpace(SelectedNotationType);
        }

        private void UpdateSize(object parameter)
        {
            // Validate if a size is selected  and input is valid
            if (SelectedSize is null)
            {
                Message = "Please select a size to update.";
                MessageType = MessageType.Error;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewSizeValue) || string.IsNullOrWhiteSpace(SelectedNotationType))
            {
                Message = "Please enter valid size value and notation type.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                if (SelectedSize != null)
                {
                    SelectedSize.SizeValue = NewSizeValue;
                    SelectedSize.NotationType = SelectedNotationType;
                    _sizeRepository.Update(SelectedSize);
                    LoadData(); // Refresh list
                    Message = "Size updated successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error updating size: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void DeleteSize(object parameter)
        {
            try
            {
                if (SelectedSize != null)
                {
                    _sizeRepository.Delete(SelectedSize.SizeId);
                    LoadData(); // Refresh list
                    SelectedSize = null; // Clear selection
                    Message = "Size deleted successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error deleting size: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanUpdateOrDeleteSize(object parameter)
        {
            return SelectedSize != null;
        }

        private void ClearSelection(object parameter)
        {
            SelectedSize = null;
            NewSizeValue = string.Empty;
            SelectedNotationType = "Americana";
            Message = string.Empty;
            MessageType = MessageType.None;
        }

        private void UpdateSizeFields()
        {
            if (SelectedSize != null)
            {
                NewSizeValue = SelectedSize.SizeValue;
                SelectedNotationType = SelectedSize.NotationType;
            }
            else
            {
                NewSizeValue = string.Empty;
                SelectedNotationType = "Americana";
            }
        }
    }
}
