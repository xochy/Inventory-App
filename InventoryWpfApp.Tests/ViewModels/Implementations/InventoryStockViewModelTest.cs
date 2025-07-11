using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Implementations;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWpfApp.Tests.ViewModels.Implementations
{
    public class InventoryStockViewModelTest
    {
        [Fact]
        public void UpdateInventoryStock_ValidData_CallsRepositoryUpdateAndRefreshesData()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();
            
            var originalStock = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 10,
                MinStockLimit = 5
            };
            var updatedStock = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 20,
                MinStockLimit = 10
            };

            // Setup the repository mock:
            mockInventoryStockRepository.Setup(repo => repo.GetAll()).Returns(new List<InventoryStock> { originalStock });

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            inventoryStockViewModel.SelectedStockItem = originalStock; // Set the stock to be updated

            // Set the properties to be updated
            inventoryStockViewModel.QuantityInput = updatedStock.CurrentQuantity.ToString();
            inventoryStockViewModel.MinStockLimitInput = updatedStock.MinStockLimit.ToString();

            // Act
            inventoryStockViewModel.UpdateStockCommand.Execute(null);

            // Assert
            // 1. Verify that the repository's Update method was called exactly once
            //    with an Employee object that has the updated QuantityInput and MinStockLimitInput.
            mockInventoryStockRepository.Verify(repo => repo.Update(It.Is<InventoryStock>(
                s => s.InventoryStockId == originalStock.InventoryStockId &&
                     s.ProductId == updatedStock.ProductId &&
                     s.SizeId == updatedStock.SizeId &&
                     s.CurrentQuantity == updatedStock.CurrentQuantity &&
                     s.MinStockLimit == updatedStock.MinStockLimit)), Times.Once);

            // 2. Verify that LoadData() was called to refresh the UI
            Assert.Equal("Stock item updated successfully.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void UpdateInventoryStock_NoInventoryStockSelected_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Act
            inventoryStockViewModel.UpdateStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Update method was never called
            mockInventoryStockRepository.Verify(repo => repo.Update(It.IsAny<InventoryStock>()), Times.Never);

            // Verify that the message is not set to success
            Assert.NotEqual("Stock item updated successfully.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void UpdateInventoryStock_InvalidQuantity_DoesNotCallRepositoryUpdateAndShowsErrorMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();
            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );
            // Set the properties to be invalid
            inventoryStockViewModel.SelectedStockItem = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 10,
                MinStockLimit = 5
            };

            inventoryStockViewModel.QuantityInput = "invalid"; // Non-numeric input

            // Act
            inventoryStockViewModel.UpdateStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Update method was never called
            mockInventoryStockRepository.Verify(repo => repo.Update(It.IsAny<InventoryStock>()), Times.Never);
            // Verify that the message is set to an error message
            Assert.Equal("Invalid current quantity. Must be a non-negative number.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void UpdateInventoryStock_InvalidMinStockLimit_DoesNotCallRepositoryUpdateAndShowsErrorMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );
            // Set the properties to be invalid
            inventoryStockViewModel.SelectedStockItem = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 10,
                MinStockLimit = 5
            };

            inventoryStockViewModel.MinStockLimitInput = "invalid"; // Non-numeric input

            // Act
            inventoryStockViewModel.UpdateStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Update method was never called
            mockInventoryStockRepository.Verify(repo => repo.Update(It.IsAny<InventoryStock>()), Times.Never);
            // Verify that the message is set to an error message
            Assert.Equal("Invalid min stock limit. Must be a non-negative number.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void AddInventoryStock_ValidData_CallsRepositoryAddAndRefreshesData()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Set the properties to be added
            inventoryStockViewModel.SelectedProductId = 1;
            inventoryStockViewModel.SelectedSizeId = 1;
            inventoryStockViewModel.QuantityInput = "15";
            inventoryStockViewModel.MinStockLimitInput = "5";

            // Act
            inventoryStockViewModel.AddOrUpdateStockCommand.Execute(null);

            // Assert 
            // Verify that the repository's Add method was called exactly once
            mockInventoryStockRepository.Verify(repo => repo.AddOrUpdateStock(It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()), Times.Once);

            // Verify that LoadData() was called to refresh the UI
            Assert.Equal("Stock added/updated successfully.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void AddInventoryStock_InvalidQuantity_DoesNotCallRepositoryAddAndShowsErrorMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();
            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Set the properties to be invalid
            inventoryStockViewModel.SelectedProductId = 1;
            inventoryStockViewModel.SelectedSizeId = 1;
            inventoryStockViewModel.QuantityInput = "invalid"; // Non-numeric input
            inventoryStockViewModel.MinStockLimitInput = "5";

            // Act
            inventoryStockViewModel.AddOrUpdateStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Add method was never called
            mockInventoryStockRepository.Verify(repo => repo.AddOrUpdateStock(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()), Times.Never);

            // Verify that the message is set to an error message
            Assert.Equal("Invalid quantity. Must be a positive number.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void AddInventoryStock_InvalidMinStockLimit_DoesNotCallRepositoryAddAndShowsErrorMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();
            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Set the properties to be invalid
            inventoryStockViewModel.SelectedProductId = 1;
            inventoryStockViewModel.SelectedSizeId = 1;
            inventoryStockViewModel.QuantityInput = "15";
            inventoryStockViewModel.MinStockLimitInput = "invalid"; // Non-numeric input

            // Act
            inventoryStockViewModel.AddOrUpdateStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Add method was never called
            mockInventoryStockRepository.Verify(repo => repo.AddOrUpdateStock(
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>(), 
                It.IsAny<int>()), Times.Never);

            // Verify that the message is set to an error message
            Assert.Equal("Invalid min stock limit. Must be a non-negative number.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void DeleteInventoryStock_ValidSelection_CallsRepositoryDeleteAndRefreshesData()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();
            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Set the stock item to be deleted
            var stockToDelete = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 10,
                MinStockLimit = 5
            };

            inventoryStockViewModel.SelectedStockItem = stockToDelete;

            // Act
            inventoryStockViewModel.DeleteStockCommand.Execute(null);

            // Assert
            // Verify that the repository's Delete method was called exactly once with the correct ID
            mockInventoryStockRepository.Verify(repo => repo.Delete(stockToDelete.InventoryStockId), Times.Once);

            // Verify that LoadData() was called to refresh the UI
            Assert.Equal("Stock item deleted successfully.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void DeleteInventoryStock_NoSelection_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Act
            inventoryStockViewModel.DeleteStockCommand.Execute(null);
            
            // Assert
            // Verify that the repository's Delete method was never called
            mockInventoryStockRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);
            
            // Verify that the message is not set to success
            Assert.NotEqual("Stock item deleted successfully.", inventoryStockViewModel.Message);
        }

        [Fact]
        public void ClearSelection_ClearsSelectedInventoryStockAndResetsInputFields()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Set some initial values
            inventoryStockViewModel.SelectedStockItem = new InventoryStock
            {
                InventoryStockId = 1,
                ProductId = 1,
                SizeId = 1,
                CurrentQuantity = 10,
                MinStockLimit = 5
            };
            inventoryStockViewModel.QuantityInput = "10";
            inventoryStockViewModel.MinStockLimitInput = "5";

            // Act
            inventoryStockViewModel.ClearSelectionCommand.Execute(null);

            // Assert
            Assert.Null(inventoryStockViewModel.SelectedStockItem);
            Assert.Equal(string.Empty, inventoryStockViewModel.QuantityInput);
            Assert.Equal(string.Empty, inventoryStockViewModel.MinStockLimitInput);
        }

        [Fact]
        public void LoadData_CallsRepositoryAndPopulatesInventoryStockProductsAndSizes()
        {
            // Arrange
            var mockInventoryStockRepository = new Mock<IInventoryStockRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockSizeRepository = new Mock<ISizeRepository>();

            // Setup the repository mocks to return some data
            var stockItems = new List<InventoryStock>
            {
                new InventoryStock { InventoryStockId = 1, ProductId = 1, SizeId = 1, CurrentQuantity = 10, MinStockLimit = 5 },
                new InventoryStock { InventoryStockId = 2, ProductId = 2, SizeId = 2, CurrentQuantity = 20, MinStockLimit = 10 }
            };
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", Description = "Description 1", ApplicabilityType = "Administrativo" },
                new Product { ProductId = 2, Name = "Product 2", Description = "Description 2", ApplicabilityType = "Sindicalizado" }
            };
            var sizes = new List<Size>
            {
                new Size { SizeId = 1, SizeValue = "L", NotationType = "Americana" },
                new Size { SizeId = 2, SizeValue = "44", NotationType = "Mexicana" }
            };

            mockInventoryStockRepository.Setup(repo => repo.GetStockDetails()).Returns(stockItems);
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(products);
            mockSizeRepository.Setup(repo => repo.GetAll()).Returns(sizes);

            var inventoryStockViewModel = new InventoryStockViewModel(
                mockInventoryStockRepository.Object,
                mockProductRepository.Object,
                mockSizeRepository.Object
            );

            // Act
            // LoadData(); is called in the constructor, so we don't need to call it explicitly here.

            // Assert
            Assert.Equal(2, inventoryStockViewModel.StockItems.Count);
            Assert.Equal(2, inventoryStockViewModel.Products.Count);
            Assert.Equal(2, inventoryStockViewModel.Sizes.Count);
        }
    }
}
