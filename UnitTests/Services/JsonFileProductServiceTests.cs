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
        /// Test to verify AddRating returns false for a rating less than zero.
        /// </summary>
        [Test]
        public void AddRating_InValid_Rating_Smaller_Than_Zero_Should_Return_False()
        {
            var result = productService.AddRating("product1", -1);

            // Assert: Should return false for invalid rating.
            Assert.That(result, Is.EqualTo(false));
        }

        /// <summary>
        /// Test to verify AddRating returns false for a rating greater than five.
        /// </summary>
        [Test]
        public void AddRating_InValid_Rating_Greater_Than_Five_Should_Return_False()
        {
            var result = productService.AddRating("product1", 6);

            // Assert: Should return false for out-of-range rating.
            Assert.That(result, Is.EqualTo(false));
        }

        /// <summary>
        /// Test to verify AddRating returns false when the product ID does not exist.
        /// </summary>
        [Test]
        public void AddRating_InValid_Non_Existent_ProductId_Should_Return_False()
        {
            var result = productService.AddRating("NonExistentProductId", 3);

            // Assert: Should return false for non-existent product ID.
            Assert.That(result, Is.EqualTo(false));
        }

        /// <summary>
        /// Test to verify AddRating successfully adds a rating to an existing product.
        /// </summary>
        [Test]
        public void AddRating_Valid_Product_With_Rating_Should_Return_True()
        {
            var result = productService.AddRating("product1", 5);
            var updatedProduct = productService.GetAllData().First(x => x.Id == "product1");

            // Assert: Should return true and update the product's ratings.
            Assert.That(result, Is.EqualTo(true));
            Assert.That(updatedProduct.Ratings.Length, Is.EqualTo(3));  // Original 2 ratings + 1 new rating.
            Assert.That(updatedProduct.Ratings.Last(), Is.EqualTo(5));
        }

        /// <summary>
        /// Test to verify AddRating creates a new ratings array if it was previously null.
        /// </summary>
        [Test]
        public void AddRating_When_Ratings_Null_Should_Create_New_Array_And_Return_True()
        {
            var result = productService.AddRating("product2", 4);
            var updatedData = productService.GetAllData().First(x => x.Id == "product2");

            // Assert: Should create a new array and add the rating.
            Assert.That(result, Is.EqualTo(true));
            Assert.That(updatedData, Is.Not.Null);
            Assert.That(updatedData.Ratings.Length, Is.EqualTo(1));
            Assert.That(updatedData.Ratings[0], Is.EqualTo(4));
        }

        /// <summary>
        /// Test to verify UpdateData successfully updates an existing product's details.
        /// </summary>
        [Test]
        public void UpdateData_Valid_Should_Update_And_Return_Product()
        {
            var data = productService.GetAllData().First();
            data.Title = "Updated Title";

            var result = productService.UpdateData(data);

            // Assert: Should update the product title and return the updated product.
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
        }

        /// <summary>
        /// Test to verify DeleteData successfully removes a product and returns it.
        /// </summary>
        [Test]
        public void DeleteData_Valid_ProductId_Should_Remove_And_Return_Product()
        {
            // Arrange: Add a new product to mock data for testing deletion.
            var newProduct = new ProductModel { Id = "delete_product", Title = "To Be Deleted" };
            mockProducts.Add(newProduct);

            // Reinitialize service with updated mock data.
            productService = new JsonFileProductService(new Mock<IWebHostEnvironment>().Object, mockProducts);

            var result = productService.DeleteData("delete_product");

            // Assert: Should successfully delete the product and return it.
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("delete_product"));
            Assert.That(productService.GetAllData().Any(x => x.Id == "delete_product"), Is.False);
        }
    }
}
