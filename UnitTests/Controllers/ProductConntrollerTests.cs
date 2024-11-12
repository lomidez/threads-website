using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using ContosoCrafts.WebSite.Controllers;

namespace UnitTests.Controllers
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<JsonFileProductService> mockProductService;
        private ProductsController productsController;

        [SetUp]
        public void Setup()
        {
            // Initialize the mock service
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, null);

            // Set up the mock to return a predefined list of products for GetAllData
            var mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" },
                new ProductModel { Id = "2", Title = "Product2" }
            };

            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);

            // Instantiate the controller with the mocked service
            productsController = new ProductsController(mockProductService.Object);
        }

        [Test]
        public void Get_Should_Return_All_Products()
        {
            // Act: Call the Get method
            var result = productsController.Get();

            // Assert: Check that the result matches the mock data
            Assert.That(result, Is.Not.Null, "Get should return a collection of products.");
            Assert.That(result.Count(), Is.EqualTo(2), "The number of products returned should match the mock data.");

            // Verify that GetAllData was called exactly once
            mockProductService.Verify(service => service.GetAllData(), Times.Once);
        }
    }
}
