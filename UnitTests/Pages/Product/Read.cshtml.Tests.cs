using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;

namespace UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the Read page model, which handles the retrieval and display of products.
    /// </summary>
    [TestFixture]
    public class ReadTests
    {
        // Mock service for simulating JsonFileProductService without hitting external dependencies
        private Mock<JsonFileProductService> mockProductService;

        // Instance of ReadModel that will be tested
        private ReadModel readPage;

        /// <summary>
        /// Initializes the required mock objects and sets up the ReadModel for testing.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Mock the web host environment for the JsonFileProductService dependency
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            // Initialize the mock product service with strict behavior to ensure all calls are expected
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, mockWebHostEnvironment.Object);

            // Initialize the ReadModel using the mock service
            readPage = new ReadModel(mockProductService.Object);
        }

        /// <summary>
        /// Test to verify that a valid product ID returns the expected product.
        /// </summary>
        [Test]
        public void OnGet_Valid_ProductId_Should_Return_Product()
        {
            // Arrange: Set up the mock service to return a product list containing a product with Id "1"
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Call the OnGet method with a valid product Id "1"
            readPage.OnGet("1");

            // Assert: Verify that the selected product is not null and has the correct title
            Assert.That(readPage.SelectedProduct, Is.Not.Null);
            Assert.That(readPage.SelectedProduct.Title, Is.EqualTo("Product1"));
        }

        /// <summary>
        /// Test to verify that an invalid product ID returns a null product.
        /// </summary>
        [Test]
        public void OnGet_InValid_ProductId_Should_Return_Null()
        {
            // Arrange: Set up the mock service to return a product list without the target Id "invalid"
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Attempt to retrieve a product with a non-existent Id "invalid"
            readPage.OnGet("invalid");

            // Assert: Verify that no product was found and SelectedProduct is null
            Assert.That(readPage.SelectedProduct, Is.Null);
        }

        /// <summary>
        /// Test to check that when there are no products available, the selected product is null.
        /// </summary>
        [Test]
        public void OnGet_No_Products_Available_Should_Return_Null()
        {
            // Arrange: Set up the mock service to return an empty list, simulating no products
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());

            // Act: Attempt to fetch a product when the product list is empty
            readPage.OnGet("1");

            // Assert: Verify that SelectedProduct is null since there are no products to retrieve
            Assert.That(readPage.SelectedProduct, Is.Null);
        }
    }
}
