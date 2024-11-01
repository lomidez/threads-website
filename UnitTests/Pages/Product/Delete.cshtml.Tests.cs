using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Pages.Products
{
    public class DeleteTests
    {
        #region TestSetup

        private DeleteModel pageModel;
        private JsonFileProductService productService;  // Direct instance of the service
        private List<ProductModel> mockProducts;

        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock products
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "test-id", Title = "To Be Deleted" },
                new ProductModel { Id = "another-id", Title = "Another Product" }
            };

            // Use helper method to create environment
            var mockEnvironment = CreateMockEnvironment();

            // Initialize the service with mock data
            productService = new JsonFileProductService(mockEnvironment, mockProducts);
            pageModel = new DeleteModel(productService);
        }

        // Helper to create mock environment for cleaner setup
        private IWebHostEnvironment CreateMockEnvironment()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("");
            return mockEnvironment.Object;
        }

        #endregion TestSetup

        #region OnGet

        [Test]
        public void OnGet_Valid_Id_Should_Load_Product()
        {
            // Arrange
            var productId = "test-id";

            // Act
            pageModel.OnGet(productId);

            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
        }

        [Test]
        public void OnGet_InValid_Id_Should_Not_Load_Product()
        {
            // Arrange
            var invalidId = "non-existent-id";

            // Act
            pageModel.OnGet(invalidId);

            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Null);
        }

        #endregion OnGet

        #region OnPost

        [Test]
        public void OnPost_Valid_Id_Should_Delete_Product_And_Redirect()
        {
            // Arrange
            pageModel.SelectedProduct = new ProductModel { Id = "test-id" }; // Ensure the correct ID

            // Act
            var result = pageModel.OnPost(); // Call OnPost to delete the product

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>()); // Check if redirected to Index
            Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("./Index")); // Ensure redirection

            // Ensure that the product is no longer in the data
            Assert.That(productService.GetAllData().Any(x => x.Id == "test-id"), Is.False,
                        $"Product 'test-id' was not deleted."); // Confirm deletion
        }

        [Test]
        public void OnPost_InValid_Id_Should_Return_NotFound()
        {
            // Arrange
            pageModel.SelectedProduct = new ProductModel { Id = "non-existent-id" };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public void OnPost_InValid_NullSelectedProduct_Should_Return_NotFound()
        {
            // Arrange
            pageModel.SelectedProduct = null; // Simulate an uninitialized SelectedProduct

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>(), "OnPost should return NotFound if SelectedProduct is null.");
        }

        #endregion OnPost
    }
}

