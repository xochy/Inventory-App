using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Base;
using InventoryWpfApp.ViewModels.Base.Enums;
using InventoryWpfApp.ViewModels.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace InventoryWpfApp.ViewModels.Implementations
{
    public class EmployeeViewModel : BaseViewModel
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IGroupRepository _groupRepository;

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

        private ObservableCollection<Group> _groups;
        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set
            {
                _groups = value;
                OnPropertyChanged();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged();
                UpdateEmployeeFields();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private string _newEmployeeName;
        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private int _selectedGroupId;
        public int SelectedGroupId
        {
            get => _selectedGroupId;
            set
            {
                _selectedGroupId = value;
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

        public ICommand AddEmployeeCommand { get; private set; }
        public ICommand UpdateEmployeeCommand { get; private set; }
        public ICommand DeleteEmployeeCommand { get; private set; }
        public ICommand ClearSelectionCommand { get; private set; }

        public EmployeeViewModel(IEmployeeRepository employeeRepository, IGroupRepository groupRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _groupRepository = groupRepository ?? throw new ArgumentNullException(nameof(groupRepository));

            LoadData();
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AddEmployeeCommand = new RelayCommand(AddEmployee, CanAddEmployee);
            UpdateEmployeeCommand = new RelayCommand(UpdateEmployee, CanUpdateOrDeleteEmployee);
            DeleteEmployeeCommand = new RelayCommand(DeleteEmployee, CanUpdateOrDeleteEmployee);
            ClearSelectionCommand = new RelayCommand(ClearSelection);
        }

        private void LoadData()
        {
            try
            {
                Employees = new ObservableCollection<Employee>(_employeeRepository.GetAllWithGroupAndType());
                Groups = new ObservableCollection<Group>(_groupRepository.GetAllWithEmployeeType());
            }
            catch (Exception ex)
            {
                Message = $"Error loading data: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void AddEmployee(object parameter)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(NewEmployeeName) || SelectedGroupId <= 0)
            {
                Message = "Please enter a valid employee name and select a group.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                var newEmployee = new Employee { Name = NewEmployeeName, GroupId = SelectedGroupId };
                _employeeRepository.Add(newEmployee);
                LoadData(); // Refresh list
                NewEmployeeName = string.Empty;
                SelectedGroupId = 0; // Reset ComboBox
                Message = "Employee added successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error adding employee: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanAddEmployee(object parameter)
        {
            return !string.IsNullOrWhiteSpace(NewEmployeeName) && SelectedGroupId > 0;
        }

        private void UpdateEmployee(object parameter)
        {
            // Validate if an employee is selected and input is valid
            if (SelectedEmployee is null)
            {
                Message = "Please select an employee to update.";
                MessageType = MessageType.Error;
                return;
            }

            if (string.IsNullOrWhiteSpace(NewEmployeeName) || SelectedGroupId <= 0)
            {
                Message = "Please enter a valid employee name and select a group.";
                MessageType = MessageType.Error;
                return;
            }

            try
            {
                // Update the selected employee
                SelectedEmployee.Name = NewEmployeeName;
                SelectedEmployee.GroupId = SelectedGroupId;

                _employeeRepository.Update(SelectedEmployee);
                LoadData(); // Refresh list

                Message = "Employee updated successfully.";
                MessageType = MessageType.Success;
            }
            catch (Exception ex)
            {
                Message = $"Error updating employee: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private void DeleteEmployee(object parameter)
        {
            try
            {
                if (SelectedEmployee != null)
                {
                    _employeeRepository.Delete(SelectedEmployee.EmployeeId);
                    LoadData(); // Refresh list
                    SelectedEmployee = null; // Clear selection
                    Message = "Employee deleted successfully.";
                    MessageType = MessageType.Success;
                }
            }
            catch (Exception ex)
            {
                Message = $"Error deleting employee: {ex.Message}";
                MessageType = MessageType.Error;
            }
        }

        private bool CanUpdateOrDeleteEmployee(object parameter)
        {
            return SelectedEmployee != null;
        }

        private void ClearSelection(object parameter)
        {
            SelectedEmployee = null;
            NewEmployeeName = string.Empty;
            SelectedGroupId = 0;
            Message = string.Empty;
            MessageType = MessageType.None;
        }

        private void UpdateEmployeeFields()
        {
            if (SelectedEmployee != null)
            {
                NewEmployeeName = SelectedEmployee.Name;
                SelectedGroupId = SelectedEmployee.GroupId;
            }
            else
            {
                NewEmployeeName = string.Empty;
                SelectedGroupId = 0;
            }
        }
    }
}
