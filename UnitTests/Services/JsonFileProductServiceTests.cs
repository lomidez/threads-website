using System.Linq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Product.AddRating
{
    /// <summary>
    /// Unit tests for the JsonFileProductService class, focusing on AddRating, UpdateData, and DeleteData methods.
    /// </summary>
    [TestFixture]
    public class JsonFileProductServiceTests
    {
        // Instance of JsonFileProductService to be tested.
        private JsonFileProductService productService;

        // Mock list of products used for testing.
        private List<ProductModel> mockProducts;

        /// <summary>
        /// Initializes mock data and sets up the testing environment before each test.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Arrange: Initialize mock products for isolated, in-memory testing.
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "product1", Title = "Test Product 1", Ratings = new int[] { 3, 4 } },
                new ProductModel { Id = "product2", Title = "Test Product 2", Ratings = null }
            };

            // Set up a mock IWebHostEnvironment for dependency injection.
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("");

            // Initialize JsonFileProductService with mock environment and mock products.
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);
        }

        /// <summary>
        /// Test to verify AddRating returns false when the product ID is null.
        /// </summary>
        [Test]
        public void AddRating_InValid_Product_Null_Should_Return_False()
        {
            var result = productService.AddRating(null, 1);

            // Assert: Should return false for null product ID.
            Assert.That(result, Is.EqualTo(false));
        }

        /// <summary>
        /// Test to verify AddRating returns false when the product ID is an empty string.
        /// </summary>
        [Test]
        public void AddRating_InValid_Product_Empty_Should_Return_False()
        {
            var result = productService.AddRating("", 1);

            // Assert: Should return false for empty product ID.
            Assert.That(result, Is.EqualTo(false));
        }

        /// <summary>
        /// Test to verify AddRating returns false for a rating less than zero
