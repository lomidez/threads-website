using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Products
{
    /// <summary>
    /// Unit test class for testing the CreateModel page functionality.
    /// </summary>
    public class CreateTests
    {
        // Instance of the CreateModel page being tested.
        private CreateModel pageModel;

        // Mock instance of the JsonFileProductService.
        private Mock<JsonFileProductService> mockProductService;

        // Mock list of ProductModel used for testing.
        private List<ProductModel> mockProducts;

        /// <summary>
        /// Initializes the test setup by creating mock services and product data.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Set up a mock product list with an existing product.
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "existing-id", Title = "Existing Product" }
            };

            // Set up mock environment and product service.
            mockProductService = new Mock<JsonFileProductService>(new Mock<IWebHostEnvironment>().Object);
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);

            // Initialize the CreateModel page with the mock product service.
            pageModel = new CreateModel(mockProductService.Object);

            // Set up TempData for the CreateModel page.
            pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        /// <summary>
        /// Test to verify that posting a product with a duplicate ID returns a PageResult with an error message.
        /// </summary>
        [Test]
        public void OnPost_valid_DuplicateId_Should_Return_Page_With_Error()
        {
            // Arrange - Set up a new product with an ID that already exists in the mock product list.
            pageModel.NewProduct = new ProductModel { Id = "existing-id", Title = "New Product" };

            // Act - Call the OnPost method.
            var result = pageModel.OnPost();

            // Assert - Verify that the result is a PageResult and an error is added to the ModelState.
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.GreaterThan(0));

            // Verify that the CreateData method was not called since the ID is a duplicate.
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Never);
        }

        /// <summary>
        /// Test to verify that when CreateData fails, a PageResult with an error is returned.
        /// </summary>
        [Test]
        public void OnPost_CreateDataFails_Should_Return_Page_With_Error()
        {
            // Arrange - Simulate a failure in saving the new product data.
            pageModel.NewProduct = new ProductModel { Id = "new-id", Title = "New Product" };
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(false);

            // Act - Call the OnPost method.
            var result = pageModel.OnPost();

            // Assert - Verify that the result is a PageResult and an error is added to the ModelState.
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState[string.Empty].Errors.Count, Is.GreaterThan(0));

            // Verify that the CreateData method was called exactly once.
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Once);
        }
    }
}
