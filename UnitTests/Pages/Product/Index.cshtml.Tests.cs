using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using System;
using Microsoft.AspNetCore.Hosting;

namespace UnitTests.Pages.Product
{
    public class IndexTests
    {
        private IndexModel pageModel;
        private Mock<JsonFileProductService> mockProductService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock products
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product 1", Category = "Electronics", Size = "Large", Color = "Red", Material = new List<string> { "Metal" }, Style = new List<string> { "Modern" } },
                new ProductModel { Id = "2", Title = "Product 2", Category = "Clothing", Size = "Medium", Color = "Blue", Material = new List<string> { "Cotton" }, Style = new List<string> { "Casual" } },
                new ProductModel { Id = "3", Title = "Product 3", Category = "Electronics", Size = "Small", Color = "Green", Material = new List<string> { "Plastic" }, Style = new List<string> { "Minimalist" } }
            };

            // Mock the product service
            mockProductService = new Mock<JsonFileProductService>(Mock.Of<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>());
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);

            // Initialize the IndexModel with a mock logger and the mocked product service
            var mockLogger = Mock.Of<ILogger<IndexModel>>();
            pageModel = new IndexModel(mockLogger, mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        [Test]
        public void OnGet_Valid_Get_Products_Should_Return_All_Products()
        {
            // Act
            pageModel.OnGet(null);

            // Assert
            Assert.That(pageModel.Products, Is.EquivalentTo(mockProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(3));
        }

        [Test]
        public void OnGet_Valid_CategoryTag_Should_Filter_Products_By_Category()
        {
            // Act
            pageModel.OnGet("Electronics");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Category.Equals("Electronics", System.StringComparison.OrdinalIgnoreCase));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(2));
        }

        [Test]
        public void OnGet_Valid_SizeTag_Should_Filter_Products_By_Size()
        {
            // Act
            pageModel.OnGet("Large");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Size.Equals("Large", System.StringComparison.OrdinalIgnoreCase));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(1));
        }

        [Test]
        public void OnGet_Valid_ColorTag_Should_Filter_Products_By_Color()
        {
            // Act
            pageModel.OnGet("Red");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Color.Equals("Red", System.StringComparison.OrdinalIgnoreCase));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(1));
        }

        [Test]
        public void OnGet_Valid_MaterialTag_Should_Filter_Products_By_Material()
        {
            // Act
            pageModel.OnGet("Metal");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Material.Any(m => m.Equals("Metal", System.StringComparison.OrdinalIgnoreCase)));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(1));
        }

        [Test]
        public void OnGet_Valid_StyleTag_Should_Filter_Products_By_Style()
        {
            // Act
            pageModel.OnGet("Modern");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Style.Any(s => s.Equals("Modern", System.StringComparison.OrdinalIgnoreCase)));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(1));
        }


        [Test]
        public void Constructor_InValid_Logger_Null_Should_Throw_ArgumentNullException()
        {
            // Arrange
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            JsonFileProductService productService = new JsonFileProductService(mockEnvironment.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new IndexModel(null, productService));
        }

        [Test]
        public void Constructor_Invalid_ProductService_Null_Should_Throw_ArgumentNullException()
        {
            // Arrange
            var mockLogger = Mock.Of<ILogger<IndexModel>>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new IndexModel(mockLogger, null));
        }

        [Test]
        public void Constructor_Valid_Parameters_Should_Initialize_IndexModel()
        {
            // Arrange
            var mockLogger = Mock.Of<ILogger<IndexModel>>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            JsonFileProductService productService = new JsonFileProductService(mockEnvironment.Object);

            // Act
            var indexModel = new IndexModel(mockLogger, productService);

            // Assert
            Assert.That(indexModel, Is.Not.Null);
            Assert.That(indexModel.ProductService, Is.EqualTo(productService));
            Assert.That(indexModel.ProductService, Is.TypeOf<JsonFileProductService>());
        }

    }
}

