using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitTests.Pages.Products
{
    public class DeleteTests
    {
        private DeleteModel pageModel;
        private Mock<JsonFileProductService> mockProductService;

        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock environment and service
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            // Initialize DeleteModel with the mocked JsonFileProductService
            pageModel = new DeleteModel(mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Test]
        public void OnPost_Null_SelectedProduct_Should_Return_NotFound()
        {
            // Arrange: Set SelectedProduct to null
            pageModel.SelectedProduct = null;

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));
            mockProductService.Verify(service => service.DeleteData(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnPost_Empty_SelectedProductId_Should_Return_NotFound()
        {
            // Arrange: Set SelectedProduct with an empty Id
            pageModel.SelectedProduct = new ProductModel { Id = string.Empty };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));
            mockProductService.Verify(service => service.DeleteData(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnPost_Whitespace_SelectedProductId_Should_Return_NotFound()
        {
            // Arrange: Set SelectedProduct with a whitespace Id
            pageModel.SelectedProduct = new ProductModel { Id = "   " };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));
            mockProductService.Verify(service => service.DeleteData(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnPost_NonExistent_Id_Should_Return_NotFound()
        {
            // Arrange
            var nonExistentProductId = "non-existent-id";
            pageModel.SelectedProduct = new ProductModel { Id = nonExistentProductId };

            // Mock DeleteData to return null for non-existent product ID
            mockProductService.Setup(service => service.DeleteData(nonExistentProductId)).Returns((ProductModel)null);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));
            mockProductService.Verify(service => service.DeleteData(nonExistentProductId), Times.Once);
        }

        [Test]
        public void OnPost_Valid_Id_Should_Delete_Product_And_Redirect()
        {
            // Arrange
            var validProductId = "valid-id";
            var product = new ProductModel { Id = validProductId, Title = "Valid Product" };
            pageModel.SelectedProduct = product;

            // Mock DeleteData to return the deleted product
            mockProductService.Setup(service => service.DeleteData(validProductId)).Returns(product);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("./Index"));
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Product successfully deleted."));
            mockProductService.Verify(service => service.DeleteData(validProductId), Times.Once);
        }







        [Test]
        public void OnGet_Valid_Id_Should_Load_Product_Details()
        {
            // Arrange
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = new List<string> { "Wood", "Metal" },
                Style = new List<string> { "Modern", "Rustic" }
            };
            // Mock GetAllData to return a list containing the selected product
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            // Act
            pageModel.OnGet(productId);
            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(pageModel.Material, Is.EqualTo("Wood, Metal"));
            Assert.That(pageModel.Style, Is.EqualTo("Modern, Rustic"));
        }
        [Test]
        public void OnGet_Invalid_Id_Should_Not_Load_Product()
        {
            // Arrange
            var invalidProductId = "invalid-id";
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());
            // Act
            pageModel.OnGet(invalidProductId);
            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Null);
            Assert.That(pageModel.Material, Is.Null);
            Assert.That(pageModel.Style, Is.Null);
        }
        [Test]
        public void OnGet_Valid_Id_With_Null_Material_And_Style_Should_Set_Empty_Strings()
        {
            // Arrange
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = null,  // Material is null
                Style = null      // Style is null
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            // Act
            pageModel.OnGet(productId);
            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(pageModel.Material, Is.EqualTo(string.Empty)); // Should be an empty string
            Assert.That(pageModel.Style, Is.EqualTo(string.Empty));    // Should be an empty string
        }


    }
}
