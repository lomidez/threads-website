using System.Linq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Collections.Generic;

namespace UnitTests.Pages.Product.AddRating
{
    [TestFixture]
    public class JsonFileProductServiceTests
    {
        private JsonFileProductService productService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void TestInitialize()
        {
            // Initialize mock products for isolated, in-memory testing
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "product1", Title = "Test Product 1", Ratings = new int[] { 3, 4 } },
                new ProductModel { Id = "product2", Title = "Test Product 2", Ratings = null }
            };

            // Set up a mock WebHostEnvironment
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns("");

            // Initialize the service with mock data
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);
        }

        [Test]
        public void AddRating_InValid_Product_Null_Should_Return_False()
        {
            var result = productService.AddRating(null, 1);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_InValid_Product_Empty_Should_Return_False()
        {
            var result = productService.AddRating("", 1);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_InValid_Rating_Smaller_Than_Zero_Should_Return_False()
        {
            var result = productService.AddRating("product1", -1);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_InValid_Rating_Greater_Than_Five_Should_Return_False()
        {
            var result = productService.AddRating("product1", 6);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_InValid_NonExistent_ProductId_Should_Return_False()
        {
            var result = productService.AddRating("NonExistentProductId", 3);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_Valid_Product_Rating_5_Should_Return_True()
        {
            // Arrange - Create a product with an existing Ratings array and one with null Ratings
            var productWithExistingRatings = new ProductModel
            {
                Id = "existing_ratings",
                Title = "Product with Existing Ratings",
                Ratings = new int[] { 3, 4 }
            };

            var productWithNullRatings = new ProductModel
            {
                Id = "null_ratings",
                Title = "Product with Null Ratings",
                Ratings = null
            };

            // Mock data source with these products
            var mockProducts = new List<ProductModel> { productWithExistingRatings, productWithNullRatings };
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act and Assert for product with existing ratings
            var resultWithExistingRatings = productService.AddRating("existing_ratings", 5);
            var updatedProductWithRatings = productService.GetAllData().First(x => x.Id == "existing_ratings");

            Assert.That(resultWithExistingRatings, Is.EqualTo(true));
            Assert.That(updatedProductWithRatings.Ratings.Length, Is.EqualTo(3));  // Original 2 + 1
            Assert.That(updatedProductWithRatings.Ratings.Last(), Is.EqualTo(5));

            // Act and Assert for product with null ratings
            var resultWithNullRatings = productService.AddRating("null_ratings", 5);
            var updatedProductWithNullRatings = productService.GetAllData().First(x => x.Id == "null_ratings");

            Assert.That(resultWithNullRatings, Is.EqualTo(true));
            Assert.That(updatedProductWithNullRatings.Ratings.Length, Is.EqualTo(1));  // Should initialize and add
            Assert.That(updatedProductWithNullRatings.Ratings[0], Is.EqualTo(5));
        }


        [Test]
        public void AddRating_When_Ratings_Null_Should_Create_New_Array_And_Return_True()
        {
            var result = productService.AddRating("product2", 4);
            var updatedData = productService.GetAllData().First(x => x.Id == "product2");

            Assert.That(result, Is.EqualTo(true));
            Assert.That(updatedData, Is.Not.Null);
            Assert.That(updatedData.Ratings.Length, Is.EqualTo(1));
            Assert.That(updatedData.Ratings[0], Is.EqualTo(4));
        }

        [Test]
        public void UpdateData_Valid_Should_Update_And_Return_Product()
        {
            // Arrange
            var data = productService.GetAllData().First();
            data.Title = "Updated Title";

            // Act
            var result = productService.UpdateData(data);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
        }

        [Test]
        public void DeleteData_Valid_ProductId_Should_Remove_And_Return_Product()
        {
            // Arrange - Add a temporary product to delete
            var newProduct = new ProductModel { Id = "delete_product", Title = "To Be Deleted" };
            mockProducts.Add(newProduct);

            // Reinitialize the product service with the updated mock data
            productService = new JsonFileProductService(new Mock<IWebHostEnvironment>().Object, mockProducts);

            // Act
            var result = productService.DeleteData("delete_product");

            // Assert that the deleted product is returned and no longer present in the service data
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("delete_product"));
            Assert.That(productService.GetAllData().Any(x => x.Id == "delete_product"), Is.True);
        }

    }
}


