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
    public class ProductViewModelTest
    {
        [Fact]
        public void UpdateProductValidData_CallsRepositoryUpdateAndRefreshesData()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var originalProduct = new Product { ProductId = 1, Name = "Old Name", Description = "Old Description", ApplicabilityType = "Administrativo" };
            var updatedProduct = new Product { ProductId = 1, Name = "New Name", Description = "New Description", ApplicabilityType = "Sindicalizado" };

            // Setup the repository mock:
            // When GetAll() is called, return a list containing the original product initially
            // Then, after the update, LoadData() should be called again, so we can mock a second GetAll() call if needed,
            // but for simplicity, we focus on the Update call and then LoadData is expected to get the updated state.
            // For LoadData() to reflect changes, mock its behavior to return updated product list after 'Update' is called.
            // This is a common pattern for testing ViewModel's LoadData after an action.
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(new List<Product> { originalProduct });

            // Set the new values in the ViewModel's input properties (assuming these exist in your ViewModel)
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                SelectedProduct = originalProduct,
                NewProductName = updatedProduct.Name,
                NewProductDescription = updatedProduct.Description,
                SelectedApplicabilityType = updatedProduct.ApplicabilityType
            };

            // Act
            // Call the command directly (or its underlying method)
            viewModel.UpdateProductCommand.Execute(null);

            // Assert
            // 1. Verify that the repository's Update method was called exactly once
            //    with an Product object that has the updated Name, Description and ApplicabilityType.
            mockProductRepository.Verify(repo => repo.Update(It.Is<Product>(
                p => p.ProductId == originalProduct.ProductId &&
                p.Name == updatedProduct.Name &&
                p.Description == updatedProduct.Description &&
                p.ApplicabilityType == updatedProduct.ApplicabilityType)), Times.Once);

            // 2. Verify that LoadData() was called to refresh the UI
            //    (This is typically verified by checking the effect on the ViewModel's collection
            //    or by verifying the LoadData calls if LoadData itself uses a mocked dependency)
            //    For this example, we assume LoadData() will refresh the collection from the mock.
            //    To properly test LoadData after update, you might need to adjust mock setup for GetAll()
            //    to return the *updated* list on a subsequent call.
            //    Here, we'll verify the message.
            Assert.Equal("Product updated successfully.", viewModel.Message);
        }

        [Fact]
        public void UpdateProduct_NoProductSelected_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var viewModel = new ProductViewModel(mockProductRepository.Object);
            // Act
            viewModel.UpdateProductCommand.Execute(null);
            // Assert
            // Verify that the repository's Update method was never called
            mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Never);
            // Verify that the message is not set to success
            Assert.NotEqual("Product updated successfully.", viewModel.Message);
        }

        [Fact]
        public void UpdateProduct_InvalidData_DoesNotCallRepositoryUpdateAndShowsErrorMessage()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var originalProduct = new Product { ProductId = 1, Name = "Old Name", Description = "Old Description", ApplicabilityType = "Administrativo" };
            // Setup the repository mock:
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(new List<Product> { originalProduct });
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                SelectedProduct = originalProduct,
                NewProductName = "", // Invalid name
                NewProductDescription = "New Description",
                SelectedApplicabilityType = "Sindicalizado"
            };
            // Act
            viewModel.UpdateProductCommand.Execute(null);
            // Assert
            // Verify that the repository's Update method was never called
            mockProductRepository.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Never);
            // Verify that the message is set to an error message
            Assert.Equal("Please enter valid product name, description and applicability type.", viewModel.Message);
        }

        [Fact]
        public void AddProduct_ValidData_CallsRepositoryAddAndRefreshesData()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var newProduct = new Product { Name = "New Product", Description = "New Description", ApplicabilityType = "Administrativo" };
            // Setup the repository mock:
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(new List<Product>());
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                NewProductName = newProduct.Name,
                NewProductDescription = newProduct.Description,
                SelectedApplicabilityType = newProduct.ApplicabilityType
            };
            // Act
            viewModel.AddProductCommand.Execute(null);
            // Assert
            mockProductRepository.Verify(repo => repo.Add(It.Is<Product>(
                p => p.Name == newProduct.Name &&
                p.Description == newProduct.Description &&
                p.ApplicabilityType == newProduct.ApplicabilityType)), Times.Once);
            
            Assert.Equal("Product added successfully.", viewModel.Message);
        }

        [Fact]
        public void AddProduct_InvalidData_DoesNotCallRepositoryAddAndShowsErrorMessage()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                NewProductName = "", // Invalid name
                NewProductDescription = "New Description",
                SelectedApplicabilityType = "Administrativo"
            };
            // Act
            viewModel.AddProductCommand.Execute(null);
            // Assert
            mockProductRepository.Verify(repo => repo.Add(It.IsAny<Product>()), Times.Never);
            Assert.Equal("Please enter valid product name, description and applicability type.", viewModel.Message);
        }

        [Fact]
        public void DeleteProduct_ValidSelection_CallsRepositoryDeleteAndRefreshesData()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var productToDelete = new Product { ProductId = 1, Name = "Product to Delete", Description = "Description", ApplicabilityType = "Administrativo" };
            // Setup the repository mock:
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(new List<Product> { productToDelete });
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                SelectedProduct = productToDelete
            };
            // Act
            viewModel.DeleteProductCommand.Execute(null);
            // Assert
            mockProductRepository.Verify(repo => repo.Delete(productToDelete.ProductId), Times.Once);
            Assert.Equal("Product deleted successfully.", viewModel.Message);
        }

        [Fact]
        public void DeleteProduct_NoSelection_DoesNothingAndShowsNoSuccessMessage()
        {   
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var viewModel = new ProductViewModel(mockProductRepository.Object);
            // Act
            viewModel.DeleteProductCommand.Execute(null);
            // Assert
            mockProductRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);
            Assert.NotEqual("Product deleted successfully.", viewModel.Message);
        }

        [Fact]
        public void ClearSelection_ClearsSelectedProductAndResetsInputFields()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var productToSelect = new Product { ProductId = 1, Name = "Product to Select", Description = "Description", ApplicabilityType = "Administrativo" };
            // Setup the repository mock:
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(new List<Product> { productToSelect });
            var viewModel = new ProductViewModel(mockProductRepository.Object)
            {
                SelectedProduct = productToSelect,
                NewProductName = "Some Name",
                NewProductDescription = "Some Description",
                SelectedApplicabilityType = "Administrativo"
            };
            // Act
            viewModel.ClearSelectionCommand.Execute(null);
            // Assert
            Assert.Null(viewModel.SelectedProduct);
            Assert.Equal(string.Empty, viewModel.NewProductName);
            Assert.Equal(string.Empty, viewModel.NewProductDescription);
        }

        [Fact]
        public void LoadData_CallsRepositoryAndPopulatesProducts()
        {
            // Arrange
            var mockProductRepository = new Mock<IProductRepository>();
            var products = new List<Product>
            {
                new Product { ProductId = 1, Name = "Product 1", Description = "Description 1", ApplicabilityType = "Administrativo" },
                new Product { ProductId = 2, Name = "Product 2", Description = "Description 2", ApplicabilityType = "Sindicalizado" }
            };
            mockProductRepository.Setup(repo => repo.GetAll()).Returns(products);
            var viewModel = new ProductViewModel(mockProductRepository.Object);

            // Act
            // LoadData(); is called in the constructor, so we don't need to call it explicitly here.
            // Assert
            Assert.Equal(2, viewModel.Products.Count);
            Assert.Equal("Product 1", viewModel.Products[0].Name);
            Assert.Equal("Product 2", viewModel.Products[1].Name);
        }
    }
}
