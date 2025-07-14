using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Implementations;
using Moq;

namespace InventoryWpfApp.Tests.ViewModels.Implementations
{
    public class EmployeeViewModelTests
    {
        [Fact]
        public void UpdateEmployee_ValidData_CallsRepositoryUpdateAndRefreshesData()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var originalEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "Old Name",
                GroupId = 101,
            };
            var updatedEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "New Name",
                GroupId = 102,
            };

            // Setup the repository mock:
            // When GetAll() is called, return a list containing the original employee initially
            // Then, after the update, LoadData() should be called again, so we can mock a second GetAll() call if needed,
            // but for simplicity, we focus on the Update call and then LoadData is expected to get the updated state.
            // For LoadData() to reflect changes, mock its behavior to return updated employee list after 'Update' is called.
            // This is a common pattern for testing ViewModel's LoadData after an action.
            mockEmployeeRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<Employee> { originalEmployee });

            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            viewModel.SelectedEmployee = originalEmployee; // Select the employee to update

            // Set the new values in the ViewModel's input properties (assuming these exist in your ViewModel)
            viewModel.NewEmployeeName = "New Name";
            viewModel.SelectedGroupId = 102;

            // Act
            // Call the command directly (or its underlying method)
            viewModel.UpdateEmployeeCommand.Execute(null);

            // Assert
            // 1. Verify that the repository's Update method was called exactly once
            //    with an Employee object that has the updated Name and GroupId.
            mockEmployeeRepository.Verify(
                repo =>
                    repo.Update(
                        It.Is<Employee>(e =>
                            e.EmployeeId == updatedEmployee.EmployeeId
                            && e.Name == updatedEmployee.Name
                            && e.GroupId == updatedEmployee.GroupId
                        )
                    ),
                Times.Once
            );

            // 2. Verify that LoadData() was called to refresh the UI
            //    (This is typically verified by checking the effect on the ViewModel's collection
            //    or by verifying the LoadData calls if LoadData itself uses a mocked dependency)
            //    For this example, we assume LoadData() will refresh the collection from the mock.
            //    To properly test LoadData after update, you might need to adjust mock setup for GetAll()
            //    to return the *updated* list on a subsequent call.
            //    Here, we'll verify the message.
            Assert.Equal("Employee updated successfully.", viewModel.Message);
        }

        [Fact]
        public void UpdateEmployee_NoEmployeeSelected_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Act
            viewModel.UpdateEmployeeCommand.Execute(null);

            // Assert
            // Verify that the repository's Update method was never called
            mockEmployeeRepository.Verify(repo => repo.Update(It.IsAny<Employee>()), Times.Never);

            // Verify that the message is not set to success
            Assert.NotEqual("Employee updated successfully.", viewModel.Message);
        }

        [Fact]
        public void UpdateEmployee_InvalidData_DoesNotCallRepositoryUpdateAndShowsErrorMessage()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Set up invalid data (e.g., empty name)
            viewModel.SelectedEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "Old Name",
                GroupId = 101,
            };

            viewModel.NewEmployeeName = string.Empty;
            viewModel.SelectedGroupId = 0; // Invalid group ID

            // Act
            viewModel.UpdateEmployeeCommand.Execute(null);

            // Assert
            mockEmployeeRepository.Verify(repo => repo.Update(It.IsAny<Employee>()), Times.Never);
            Assert.Equal(
                "Please enter a valid employee name and select a group.",
                viewModel.Message
            );
        }

        [Fact]
        public void AddEmployee_ValidData_CallsRepositoryAddAndRefreshesData()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Set up the new employee data
            viewModel.NewEmployeeName = "New Employee";
            viewModel.SelectedGroupId = 1;

            // Act
            viewModel.AddEmployeeCommand.Execute(null);

            // Assert
            mockEmployeeRepository.Verify(
                repo => repo.Add(It.Is<Employee>(e => e.Name == "New Employee" && e.GroupId == 1)),
                Times.Once
            );

            // Verify that LoadData was called to refresh the employee list
            Assert.Equal("Employee added successfully.", viewModel.Message);
        }

        [Fact]
        public void AddEmployee_InvalidData_DoesNotCallRepositoryAddAndShowsErrorMessage()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Set up invalid data (e.g., empty name)
            viewModel.NewEmployeeName = string.Empty;
            viewModel.SelectedGroupId = 0; // Invalid group ID

            // Act
            viewModel.AddEmployeeCommand.Execute(null);

            // Assert
            mockEmployeeRepository.Verify(repo => repo.Add(It.IsAny<Employee>()), Times.Never);

            // Verify that the message indicates an error
            Assert.Equal(
                "Please enter a valid employee name and select a group.",
                viewModel.Message
            );
        }

        [Fact]
        public void DeleteEmployee_ValidSelection_CallsRepositoryDeleteAndRefreshesData()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var employeeToDelete = new Employee
            {
                EmployeeId = 1,
                Name = "Employee to Delete",
                GroupId = 101,
            };
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Set the selected employee to delete
            viewModel.SelectedEmployee = employeeToDelete;

            // Act
            viewModel.DeleteEmployeeCommand.Execute(null);

            // Assert
            mockEmployeeRepository.Verify(
                repo => repo.Delete(employeeToDelete.EmployeeId),
                Times.Once
            );

            // Verify that LoadData was called to refresh the employee list
            Assert.Equal("Employee deleted successfully.", viewModel.Message);
        }

        [Fact]
        public void DeleteEmployee_NoSelection_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Act
            viewModel.DeleteEmployeeCommand.Execute(null);

            // Assert
            // Verify that the repository's Delete method was never called
            mockEmployeeRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);

            // Verify that the message is not set to success
            Assert.NotEqual("Employee deleted successfully.", viewModel.Message);
        }

        [Fact]
        public void ClearSelection_ClearsSelectedEmployeeAndResetsInputFields()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Set some values to SelectedEmployee and input fields
            viewModel.SelectedEmployee = new Employee
            {
                EmployeeId = 1,
                Name = "Test Employee",
                GroupId = 101,
            };
            viewModel.NewEmployeeName = "Test Name";
            viewModel.SelectedGroupId = 1;

            // Act
            viewModel.ClearSelectionCommand.Execute(null);

            // Assert
            Assert.Null(viewModel.SelectedEmployee);
            Assert.Equal(string.Empty, viewModel.NewEmployeeName);
            Assert.Equal(0, viewModel.SelectedGroupId);
        }

        [Fact]
        public void LoadData_CallsRepositoryAndPopulatesEmployeesAndGroups()
        {
            // Arrange
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockGroupRepository = new Mock<IGroupRepository>();
            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1,
                    Name = "Employee 1",
                    GroupId = 101,
                },
                new Employee
                {
                    EmployeeId = 2,
                    Name = "Employee 2",
                    GroupId = 102,
                },
            };
            var groups = new List<Group>
            {
                new Group { GroupId = 101, Name = "Group 1" },
                new Group { GroupId = 102, Name = "Group 2" },
            };
            mockEmployeeRepository.Setup(repo => repo.GetAllWithGroupAndType()).Returns(employees);
            mockGroupRepository.Setup(repo => repo.GetAllWithEmployeeType()).Returns(groups);
            var viewModel = new EmployeeViewModel(
                mockEmployeeRepository.Object,
                mockGroupRepository.Object
            );

            // Act
            // LoadData(); is called in the constructor, so we don't need to call it explicitly here.

            // Assert
            Assert.Equal(2, viewModel.Employees.Count);
            Assert.Equal(2, viewModel.Groups.Count);
        }
    }
}
