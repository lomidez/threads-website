using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Components
{
    public class ProductListTests
    {
        private JsonFileProductService productService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void Setup()
        {
            // Mock the IWebHostEnvironment dependency
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            // Initialize the product service with the mocked environment
            productService = new JsonFileProductService(mockEnvironment.Object);

            // Set up the mock product list to include "Burgundy Leather Chic Handbag"
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

            // Override the GetAllData method to return the mock product list
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);
        }

        [Test]
        public void ProductList_Valid_Default_Should_Return_Content()
        {
            // Act
            var result = productService.GetAllData();
            var containsExpectedProduct = result.Any(p => p.Title == "Burgundy Leather Chic Handbag");

            // Assert
            Assert.That(containsExpectedProduct, Is.True, "Expected product 'Burgundy Leather Chic Handbag' was not found in the product list.");
        }
    }
}

