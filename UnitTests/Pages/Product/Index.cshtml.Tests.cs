﻿using System.Collections.Generic;
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
    /// <summary>
    /// Unit tests for the <see cref="IndexModel"/> class, responsible for testing the product filtering logic in the Index page.
    /// </summary>
    public class IndexTests
    {
        private IndexModel pageModel;
        private Mock<JsonFileProductService> mockProductService;
        private List<ProductModel> mockProducts;

        /// <summary>
        /// Initializes the test environment, creating mock products and mock services.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock products
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product 1", Category = ProductCategory.Electronics, Size = ProductSize.Large, Color = "Red", Material = new List<string> { "Metal" }, Style = new List<string> { "Modern" } },
                new ProductModel { Id = "2", Title = "Product 2", Category = ProductCategory.Clothing, Size = ProductSize.Medium, Color = "Blue", Material = new List<string> { "Cotton" }, Style = new List<string> { "Casual" } },
                new ProductModel { Id = "3", Title = "Product 3", Category = ProductCategory.Electronics, Size = ProductSize.Small, Color = "Green", Material = new List<string> { "Plastic" }, Style = new List<string> { "Minimalist" } }
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

        /// <summary>
        /// Tests the OnGet method for returning all products when no category filter is applied.
        /// </summary>
        [Test]
        public void OnGet_Valid_Get_Products_Should_Return_All_Products()
        {
            // Act
            pageModel.OnGet(null);

            // Assert
            Assert.That(pageModel.Products, Is.EquivalentTo(mockProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(3));
        }

        /// <summary>
        /// Tests the OnGet method for filtering products by category.
        /// </summary>
        [Test]
        public void OnGet_Valid_CategoryTag_Should_Filter_Products_By_Category()
        {
            // Act
            pageModel.OnGet("Electronics");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Category.ToString().Equals("Electronics", System.StringComparison.OrdinalIgnoreCase));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(2));
        }

        /// <summary>
        /// Tests the OnGet method for filtering products by size.
        /// </summary>
        [Test]
        public void OnGet_Valid_SizeTag_Should_Filter_Products_By_Size()
        {
            // Act
            pageModel.OnGet("Large");

            // Assert
            var filteredProducts = mockProducts.Where(p => p.Size.ToString().Equals("Large", System.StringComparison.OrdinalIgnoreCase));
            Assert.That(pageModel.Products, Is.EquivalentTo(filteredProducts));
            Assert.That(pageModel.Products.Count(), Is.EqualTo(1));
        }

        /// <summary>
        /// Tests the OnGet method for filtering products by color.
        /// </summary>
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

        /// <summary>
        /// Tests the OnGet method for filtering products by material.
        /// </summary>
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

        /// <summary>
        /// Tests the OnGet method for filtering products by style.
        /// </summary>
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

       

        
       

        /// <summary>
        /// Tests the constructor with valid parameters to ensure the <see cref="IndexModel"/> is initialized correctly.
        /// </summary>
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
