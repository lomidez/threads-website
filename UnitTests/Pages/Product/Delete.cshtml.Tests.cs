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
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Products
{
    /// <summary>
    /// Unit test class for testing the DeleteModel page functionality.
    /// </summary>
    public class DeleteTests
    {
        // Instance of the DeleteModel page being tested.
        private DeleteModel pageModel;

        // Mock instance of the JsonFileProductService.
        private Mock<JsonFileProductService> mockProductService;

        /// <summary>
        /// Initializes the test setup by creating mock services.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock environment and product service.
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            // Initialize DeleteModel with the mocked JsonFileProductService and TempData.
            pageModel = new DeleteModel(mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        /// <summary>
        /// Test to verify that attempting to delete a non-existent product ID returns a NotFoundResult.
        /// </summary>
        [Test]
        public void OnPost_NonExistent_Id_Should_Return_NotFound()
        {
            // Arrange - Set up a non-existent product ID for deletion.
            var nonExistentProductId = "non-existent-id";
            pageModel.SelectedProduct = new ProductModel { Id = nonExistentProductId };

            // Mock DeleteData to return null for a non-existent product ID.
            mockProductService.Setup(service => service.DeleteData(nonExistentProductId)).Returns((ProductModel)null);

            // Act - Call the OnPost method.
            var result = pageModel.OnPost();

            // Assert - Verify that the result is a NotFoundResult and a notification is added to TempData.
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));

            // Verify that DeleteData was called exactly once with the non-existent ID.
            mockProductService.Verify(service => service.DeleteData(nonExistentProductId), Times.Once);
        }

        /// <summary>
        /// Test to verify that a valid product ID loads the product details successfully.
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Should_Load_Product_Details()
        {
            // Arrange - Set up a valid product ID and mock product data.
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = new List<string> { "Wood" },
                Style = new List<string> { "Modern" }
            };

            // Mock GetAllData to return a list containing the selected product.
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });

            // Act - Call the OnGet method with the valid product ID.
            pageModel.OnGet(productId);

            // Assert - Verify that the selected product and its attributes are loaded correctly.
            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(pageModel.Material, Is.EqualTo("Wood"));
            Assert.That(pageModel.Style, Is.EqualTo("Modern"));
        }

        /// <summary>
        /// Test to verify that an invalid product ID does not load any product details.
        /// </summary>
        [Test]
        public void OnGet_Invalid_Id_Should_Not_Load_Product()
        {
            // Arrange - Set up an invalid product ID and mock an empty product list.
            var invalidProductId = "invalid-id";
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());

            // Act - Call the OnGet method with the invalid product ID.
            pageModel.OnGet(invalidProductId);

            // Assert - Verify that no product or attributes are loaded.
            Assert.That(pageModel.SelectedProduct, Is.Null);
            Assert.That(pageModel.Material, Is.Null);
            Assert.That(pageModel.Style, Is.Null);
        }

        /// <summary>
        /// Test to verify that an empty SelectedProduct ID returns a NotFoundResult without attempting to delete.
        /// </summary>
        [Test]
        public void OnPost_Empty_SelectedProductId_Should_Return_NotFound()
        {
            // Arrange - Set SelectedProduct with an empty ID.
            pageModel.SelectedProduct = new ProductModel { Id = string.Empty };

            // Act - Call the OnPost method.
            var result = pageModel.OnPost();

            // Assert - Verify that the result is a NotFoundResult and no delete action is attempted.
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product."));

            // Verify that DeleteData was never called since the ID was empty.
            mockProductService.Verify(service => service.DeleteData(It.IsAny<string>()), Times.Never);
        }
    }
}
