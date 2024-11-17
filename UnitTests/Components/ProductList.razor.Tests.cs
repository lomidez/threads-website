using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Components
{
    /// <summary>
    /// Unit tests for the ProductList component.
    /// </summary>
    public class ProductListTests
    {
        // Instance of the product service under test.
        private JsonFileProductService productService;

        // Mock list of products used for testing.
        private List<ProductModel> mockProducts;

        /// <summary>
        /// Setup method to initialize mock dependencies and test data before each test run.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Mock the IWebHostEnvironment dependency to be used by the product service.
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            // Initialize the product service with the mocked environment object.
            productService = new JsonFileProductService(mockEnvironment.Object);

            // Set up the mock product list with a sample product for testing.
            mockProducts = new List<ProductModel>
            {
                new ProductModel
                {
                    Id = "handbag-unique-id",
                    Title = "Burgundy Leather Chic Handbag",
                    Description = "A chic leather handbag in a stunning burgundy color.",
                    Image = "https://example.com/path/to/image.png"
                }
            };

            // Override the GetAllData method to return the mock product list instead of reading from the file.
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);
        }

        /// <summary>
        /// Test to verify that the default product list contains the expected product.
        /// </summary>
        [Test]
        public void ProductList_Valid_Default_Should_Return_Content()
        {
            // Act: Retrieve the list of products from the product service.
            var result = productService.GetAllData();

            // Check if the product list contains the expected product with the title "Burgundy Leather Chic Handbag".
            var containsExpectedProduct = result.Any(p => p.Title == "Burgundy Leather Chic Handbag");

            // Assert: Verify that the expected product is present in the result.
            Assert.That(containsExpectedProduct, Is.True, "Expected product 'Burgundy Leather Chic Handbag' was not found in the product list.");
        }
    }
}
