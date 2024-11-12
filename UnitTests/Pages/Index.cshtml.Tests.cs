using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using ContosoCrafts.WebSite.Pages.Product;
using Microsoft.AspNetCore.Hosting;

namespace UnitTests.Pages.Product
{
    [TestFixture]
    public class IndexModelTests
    {
        private IndexModel indexModel;
        private Mock<IWebHostEnvironment> mockWebHostEnvironment;
        private Mock<JsonFileProductService> mockProductService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void Setup()
        {
            // Mock the IWebHostEnvironment dependency for JsonFileProductService
            mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockWebHostEnvironment.Setup(env => env.WebRootPath).Returns("test_path");

            // Create a real instance of JsonFileProductService using the mocked environment
            mockProductService = new Mock<JsonFileProductService>(mockWebHostEnvironment.Object);

            // Prepare mock data for GetAllData
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Test Product 1" },
                new ProductModel { Id = "2", Title = "Test Product 2" }
            };

            // Setup the GetAllData method to return mock products with strict mock behavior
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts).Verifiable();

            // Initialize the IndexModel with the mocked service
            indexModel = new IndexModel(mockProductService.Object);
        }

        [Test]
        public void OnGet_Should_Set_Products_Property()
        {
            // Act
            indexModel.OnGet();

            // Assign Products to a local variable to prevent multiple access calls
            var products = indexModel.Products;

            // Assert: Check that Products is set correctly
            Assert.That(products, Is.EqualTo(mockProducts));

            // Verify that GetAllData was called exactly once
            mockProductService.Verify(service => service.GetAllData(), Times.Once);
        }
    }
}
