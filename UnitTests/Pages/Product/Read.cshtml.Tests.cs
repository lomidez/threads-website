using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;

namespace UnitTests.Pages
{
    [TestFixture]
    public class ReadTests
    {
        private Mock<JsonFileProductService> mockProductService;
        private ReadModel readPage;

        [SetUp]
        public void SetUp()
        {
            // Mock the JsonFileProductService with strict behavior to control data
            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, mockWebHostEnvironment.Object);

            // Initialize ReadModel with the mocked JsonFileProductService
            readPage = new ReadModel(mockProductService.Object);
        }

        [Test]
        public void OnGet_Valid_ProductId_Should_Return_Product()
        {
            // Arrange: Set up the GetAllData method to return a mock product list with the target Id
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Fetch the product with Id "1"
            readPage.OnGet("1");

            // Assert: Verify the correct product was retrieved
            Assert.That(readPage.SelectedProduct, Is.Not.Null);
            Assert.That(readPage.SelectedProduct.Title, Is.EqualTo("Product1"));
        }

        [Test]
        public void OnGet_InValid_ProductId_Should_Return_Null()
        {
            // Arrange: Set up the GetAllData method to return a mock product list without the target Id
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Attempt to fetch a product with a non-existent Id "invalid"
            readPage.OnGet("invalid");

            // Assert: Verify that no product was found and SelectedProduct is null
            Assert.That(readPage.SelectedProduct, Is.Null);
        }

        [Test]
        public void OnGet_NoProductsAvailable_Should_Return_Null()
        {
            // Arrange: Set up the GetAllData method to return an empty list
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());

            // Act: Attempt to fetch any product when none are available
            readPage.OnGet("1");

            // Assert: Verify that SelectedProduct is null as there are no products
            Assert.That(readPage.SelectedProduct, Is.Null);
        }
    }
}
