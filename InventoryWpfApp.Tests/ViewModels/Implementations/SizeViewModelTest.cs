using InventoryWpfApp.Models;
using InventoryWpfApp.Repositories.Contracts;
using InventoryWpfApp.ViewModels.Implementations;
using Moq;

namespace InventoryWpfApp.Tests.ViewModels.Implementations
{
    public class SizeViewModelTest
    {
        [Fact]
        public void UpdateSize_ValidData_CallsRepositoryUpdateAndRefreshesData()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);
            var originalSize = new Size
            {
                SizeId = 1,
                SizeValue = "L",
                NotationType = "Americana",
            };
            var updatedSize = new Size
            {
                SizeId = 1,
                SizeValue = "44",
                NotationType = "Mexicana",
            };

            // Setup the repository mock:
            mockSizeRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<Size> { originalSize });

            var viewModel = new SizeViewModel(mockSizeRepository.Object);

            viewModel.SelectedSize = originalSize; // Set the size to be updated

            // Set the properties to be updated
            viewModel.NewSizeValue = updatedSize.SizeValue;
            viewModel.SelectedNotationType = updatedSize.NotationType;

            // Act
            viewModel.UpdateSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(
                repo =>
                    repo.Update(
                        It.Is<Size>(s =>
                            s.SizeId == originalSize.SizeId
                            && s.SizeValue == updatedSize.SizeValue
                            && s.NotationType == updatedSize.NotationType
                        )
                    ),
                Times.Once
            );

            Assert.Equal("Size updated successfully.", viewModel.Message);
        }

        [Fact]
        public void UpdateSize_NoSizeSelected_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);

            // Act
            sizeViewModel.UpdateSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(repo => repo.Update(It.IsAny<Size>()), Times.Never);
            Assert.Equal("Please select a size to update.", sizeViewModel.Message);
        }

        [Fact]
        public void UpdateSize_InvalidData_DoesNotCallRepositoryUpdateAndShowsErrorMessage()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);
            var originalSize = new Size
            {
                SizeId = 1,
                SizeValue = "L",
                NotationType = "Americana",
            };

            // Setup the repository mock:
            mockSizeRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<Size> { originalSize });
            var viewModel = new SizeViewModel(mockSizeRepository.Object);

            viewModel.SelectedSize = originalSize; // Set the size to be updated

            // Set invalid properties
            viewModel.NewSizeValue = ""; // Invalid size value
            viewModel.SelectedNotationType = "Invalid"; // Invalid notation type

            // Act
            viewModel.UpdateSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(repo => repo.Update(It.IsAny<Size>()), Times.Never);
            Assert.Equal("Please enter valid size value and notation type.", viewModel.Message);
        }

        [Fact]
        public void AddSize_ValidData_CallsRepositoryAddAndRefreshesData()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);
            var newSize = new Size { SizeValue = "M", NotationType = "Americana" };

            // Act
            sizeViewModel.NewSizeValue = newSize.SizeValue;
            sizeViewModel.SelectedNotationType = newSize.NotationType;
            sizeViewModel.AddSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(
                repo =>
                    repo.Add(
                        It.Is<Size>(s =>
                            s.SizeValue == newSize.SizeValue
                            && s.NotationType == newSize.NotationType
                        )
                    ),
                Times.Once
            );
            Assert.Equal("Size added successfully.", sizeViewModel.Message);
        }

        [Fact]
        public void AddSize_InvalidData_DoesNotCallRepositoryAddAndShowsErrorMessage()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);

            // Act
            sizeViewModel.NewSizeValue = ""; // Invalid size value
            sizeViewModel.SelectedNotationType = "Invalid"; // Invalid notation type
            sizeViewModel.AddSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(repo => repo.Add(It.IsAny<Size>()), Times.Never);
            Assert.Equal("Please enter valid size value and notation type.", sizeViewModel.Message);
        }

        [Fact]
        public void DeleteSize_ValidSelection_CallsRepositoryDeleteAndRefreshesData()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);
            var sizeToDelete = new Size
            {
                SizeId = 1,
                SizeValue = "L",
                NotationType = "Americana",
            };

            // Setup the repository mock:
            mockSizeRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<Size> { sizeToDelete });
            sizeViewModel.SelectedSize = sizeToDelete; // Set the size to be deleted

            // Act
            sizeViewModel.DeleteSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(repo => repo.Delete(sizeToDelete.SizeId), Times.Once);
            Assert.Equal("Size deleted successfully.", sizeViewModel.Message);
        }

        [Fact]
        public void DeleteSize_NoSelection_DoesNothingAndShowsNoSuccessMessage()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);

            // Act
            sizeViewModel.DeleteSizeCommand.Execute(null);

            // Assert
            mockSizeRepository.Verify(repo => repo.Delete(It.IsAny<int>()), Times.Never);
            Assert.NotEqual("Size deleted successfully.", sizeViewModel.Message);
        }

        [Fact]
        public void ClearSelection_ClearsSelectedSizeAndResetsInputFields()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);
            var sizeToSelect = new Size
            {
                SizeId = 1,
                SizeValue = "L",
                NotationType = "Americana",
            };

            // Setup the repository mock:
            mockSizeRepository
                .Setup(repo => repo.GetAll())
                .Returns(new List<Size> { sizeToSelect });
            sizeViewModel.SelectedSize = sizeToSelect; // Set the size to be cleared

            // Act
            sizeViewModel.ClearSelectionCommand.Execute(null);

            // Assert
            Assert.Null(sizeViewModel.SelectedSize);
            Assert.Equal(string.Empty, sizeViewModel.NewSizeValue);
        }

        [Fact]
        public void LoadData_CallsRepositoryAndPopulatesSizes()
        {
            // Arrange
            var mockSizeRepository = new Mock<ISizeRepository>();
            var sizes = new List<Size>
            {
                new Size
                {
                    SizeId = 1,
                    SizeValue = "L",
                    NotationType = "Americana",
                },
                new Size
                {
                    SizeId = 2,
                    SizeValue = "M",
                    NotationType = "Mexicana",
                },
            };
            // Setup the repository mock:
            mockSizeRepository.Setup(repo => repo.GetAll()).Returns(sizes);
            var sizeViewModel = new SizeViewModel(mockSizeRepository.Object);

            // Act
            // LoadData(); is called in the constructor, so we don't need to call it explicitly here.

            // Assert
            Assert.Equal(2, sizeViewModel.Sizes.Count);
            Assert.Contains(sizeViewModel.Sizes, s => s.SizeValue == "L");
            Assert.Contains(sizeViewModel.Sizes, s => s.SizeValue == "M");
        }
    }
}
