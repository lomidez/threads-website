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
        public void AddRating_InValid_Non_Existent_ProductId_Should_Return_False()
        {
            var result = productService.AddRating("NonExistentProductId", 3);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void AddRating_Valid_Product_With_Rating_Should_Return_True()
        {
            var result = productService.AddRating("product1", 5);
            var updatedProduct = productService.GetAllData().First(x => x.Id == "product1");

            Assert.That(result, Is.EqualTo(true));
            Assert.That(updatedProduct.Ratings.Length, Is.EqualTo(3));  // Original 2 + 1
            Assert.That(updatedProduct.Ratings.Last(), Is.EqualTo(5));
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
            var data = productService.GetAllData().First();
            data.Title = "Updated Title";

            var result = productService.UpdateData(data);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Title"));
        }

        [Test]
        public void DeleteData_Valid_ProductId_Should_Remove_And_Return_Product()
        {
            var newProduct = new ProductModel { Id = "delete_product", Title = "To Be Deleted" };
            mockProducts.Add(newProduct);

            productService = new JsonFileProductService(new Mock<IWebHostEnvironment>().Object, mockProducts);

            var result = productService.DeleteData("delete_product");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo("delete_product"));
            Assert.That(productService.GetAllData().Any(x => x.Id == "delete_product"), Is.False);
        }
    }
}
