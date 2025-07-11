using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Implementations;
using Moq;

namespace InventoryWpfApp.Tests.ViewModels.Implementations
{
    public class DeliveryViewModelTest
    {
        [Fact]
        public void RegisterDelivery_ValidData_CallsRepositoryRegisterDelivery()
        {
            // Arrange
            var mockMovementRepository = new Mock<IMovementRepository>();
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();

            var deliveryViewModel = new DeliveryViewModel(
                mockMovementRepository.Object,
                mockEmployeeRepository.Object,
                mockProductRepository.Object,
                mockInventoryStockRepository.Object
            );

            deliveryViewModel.SelectedInventoryStockId = 1;
            deliveryViewModel.SelectedEmployeeId = 1;
            deliveryViewModel.QuantityToDeliver = "10";

            // Act
            deliveryViewModel.RegisterDeliveryCommand.Execute(null);

            // Assert
            mockMovementRepository.Verify(m => m.RegisterDelivery(1, 1, 10), Times.Once);
        }

        [Fact]
        public void RegisterDelivery_InvalidQuantityToDeliver_DoesNotCallRepositoryRegisterDeliveryAndShowsErrorMessage()
        {
            // Arrange
            var mockMovementRepository = new Mock<IMovementRepository>();
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();

            var deliveryViewModel = new DeliveryViewModel(
                mockMovementRepository.Object,
                mockEmployeeRepository.Object,
                mockProductRepository.Object,
                mockInventoryStockRepository.Object
            );

            deliveryViewModel.SelectedInventoryStockId = 1;
            deliveryViewModel.SelectedEmployeeId = 1;
            deliveryViewModel.QuantityToDeliver = "invalid"; // Invalid quantity

            // Act
            deliveryViewModel.RegisterDeliveryCommand.Execute(null);

            // Assert
            mockMovementRepository.Verify(m => m.RegisterDelivery(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            Assert.Equal("Invalid quantity. Must be a positive number.", deliveryViewModel.Message);
        }

        [Fact]
        public void ClearSelection_ClearsSelectedDeliveryAndResetsInputFields()
        {
            // Arrange
            var mockMovementRepository = new Mock<IMovementRepository>();
            var mockEmployeeRepository = new Mock<IEmployeeRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();

            var deliveryViewModel = new DeliveryViewModel(
                mockMovementRepository.Object,
                mockEmployeeRepository.Object,
                mockProductRepository.Object,
                mockInventoryStockRepository.Object
            );

            deliveryViewModel.SelectedInventoryStockId = 1;
            deliveryViewModel.SelectedEmployeeId = 1;
            deliveryViewModel.QuantityToDeliver = "10";

            // Act
            deliveryViewModel.ClearDeliveryFieldsCommand.Execute(null);
            
            // Assert
            Assert.Equal(0, deliveryViewModel.SelectedInventoryStockId);
            Assert.Equal(0, deliveryViewModel.SelectedEmployeeId);
            Assert.Equal(string.Empty, deliveryViewModel.QuantityToDeliver);
        }
    }
}
