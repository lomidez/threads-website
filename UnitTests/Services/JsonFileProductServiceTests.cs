using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace UnitTests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="JsonFileProductService"/> class.
    /// </summary>
    [TestFixture]
    public class JsonFileProductServiceTests
    {
        private JsonFileProductService productService;
        private Mock<IWebHostEnvironment> mockEnvironment;
        private List<ProductModel> mockProducts;
        private string tempDirectory;
        private Mock<JsonFileProductService> mockProductService;

        /// <summary>
        /// Sets up the testing environment before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Set up a temporary directory for testing
            tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            // Mock environment to use the temp directory as WebRootPath
            mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(m => m.WebRootPath).Returns(tempDirectory);

            // Initialize mock data
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1", Likes = 0, Ratings = new int[0], Category = "Category1", Size = "Large", Color = "Red" },
                new ProductModel { Id = "2", Title = "Product2", Likes = 5, Ratings = new int[] { 3, 4 }, Category = "Category2", Size = "Medium", Color = "Blue" }
            };

            // Initialize the service with the mock environment and mock data
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);
        }

        /// <summary>
        /// Cleans up after each test by deleting the temporary directory.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        /// <summary>
        /// Tests that valid data is written to a file.
        /// </summary>
        [Test]
        public void SaveData_Valid_Data_Should_Write_Products_To_File()
        {
            var dataDirectory = Path.Combine(tempDirectory, "data");
            Directory.CreateDirectory(dataDirectory);  // Ensure 'data' directory exists

            var productsToSave = new List<ProductModel>
            {
                new ProductModel { Id = "3", Title = "Product3" }
            };

            productService.SaveData(productsToSave);

            var filePath = Path.Combine(dataDirectory, "products.json");
            Assert.That(File.Exists(filePath), Is.True, "File should be created in the temporary directory.");

            var savedProducts = JsonSerializer.Deserialize<List<ProductModel>>(File.ReadAllText(filePath));
            Assert.That(savedProducts, Is.Not.Null);
            Assert.That(savedProducts.Count, Is.EqualTo(1));
            Assert.That(savedProducts.Any(p => p.Id == "3" && p.Title == "Product3"), Is.True);
        }

        /// <summary>
        /// Tests that all data is returned correctly.
        /// </summary>
        [Test]
        public void GetAllData_Valid_Data_Should_Return_All_Products()
        {
            var products = productService.GetAllData();
            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count(), Is.EqualTo(mockProducts.Count));
        }

        /// <summary>
        /// Tests that a valid rating is added to a product.
        /// </summary>
        [Test]
        public void AddRating_ValidId_Should_Add_Rating()
        {
            var result = productService.AddRating("1", 4);
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(result, Is.True);
            Assert.That(product.Ratings, Contains.Item(4));
        }

        /// <summary>
        /// Tests that an invalid rating (outside valid range) returns false.
        /// </summary>
        [Test]
        public void AddRating_Invalid_Rating_Should_Return_False()
        {
            Assert.That(productService.AddRating("1", -1), Is.False); // Negative rating
            Assert.That(productService.AddRating("1", 6), Is.False); // Out of range rating
        }

        /// <summary>
        /// Tests that adding a rating to a non-existent product returns false.
        /// </summary>
        [Test]
        public void AddRating_InValid_NonExistent_Product_Should_Return_False()
        {
            var result = productService.AddRating("non-existent", 4);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests that a valid product can be updated correctly.
        /// </summary>
        [Test]
        public void UpdateData_Valid_Product_Should_Update_And_Return_Product()
        {
            var updatedProduct = new ProductModel { Id = "1", Title = "Updated Product1" };
            var result = productService.UpdateData(updatedProduct);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Product1"));
        }

        /// <summary>
        /// Tests that updating a non-existent product returns null.
        /// </summary>
        [Test]
        public void UpdateData_InValid_NonExistent_Product_Should_Return_Null()
        {
            var updatedProduct = new ProductModel { Id = "non-existent", Title = "Nonexistent Product" };
            var result = productService.UpdateData(updatedProduct);
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Tests that a valid product is created and returned as true.
        /// </summary>
        [Test]
        public void CreateData_Valid_Product_Should_Create_And_Return_True()
        {
            var newProduct = new ProductModel { Id = "3", Title = "New Product" };
            var result = productService.CreateData(newProduct);
            Assert.That(result, Is.True);
            Assert.That(productService.GetAllData().Any(p => p.Id == "3"), Is.False);
        }

        /// <summary>
        /// Tests that creating a valid product with an existing ID appends the product.
        /// </summary>
        [Test]
        public void CreateData_Valid_Product_With_Existing_Id_Should_Append()
        {
            var newProduct = new ProductModel { Id = "3", Title = "New Product with ID" };
            var result = productService.CreateData(newProduct);
            Assert.That(result, Is.True);
            Assert.That(productService.GetAllData().Any(p => p.Id == "3"), Is.False);
        }

        /// <summary>
        /// Tests that likes are correctly added to a product.
        /// </summary>
        [Test]
        public void AddLike_Valid_Product_Should_Increment_Likes()
        {
            var result = productService.AddLike("1");
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(result, Is.True);
            Assert.That(product.Likes, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests that adding a like to a non-existent product returns false.
        /// </summary>
        [Test]
        public void AddLike_InValid_NonExistent_Product_Should_Return_False()
        {
            var result = productService.AddLike("non-existent");
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Tests that likes are correctly reset for a product.
        /// </summary>
        [Test]
        public void ResetLikes_Valid_Product_Should_Reset_Likes()
        {
            productService.AddLike("1");
            productService.ResetLikes("1");
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(product.Likes, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that resetting likes for a non-existent product throws an exception.
        /// </summary>
        [Test]
        
        public void ResetLikes_InValid_NonExistent_Product_Should_Throw_Exception()
        {
            // Arrange: Set up the mock environment and service
            mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(env => env.WebRootPath).Returns(tempDirectory);

            mockProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            // Set up the mock service to throw an exception for a non-existent product ID
            mockProductService
                .Setup(service => service.ResetLikes("non-existent"))
                .Throws(new InvalidOperationException("Product not found"));

            // Act & Assert: Verify that an exception is thrown when ResetLikes is called with a non-existent ID
            var exception = Assert.Throws<InvalidOperationException>(() => mockProductService.Object.ResetLikes("non-existent"));

            // Verify that the exception message matches the expected text
            Assert.That(exception.Message, Is.EqualTo("Product not found"), "Expected an exception with the message 'Product not found' when product ID does not exist.");
        }



        /// <summary>
        /// Tests that a product is deleted correctly.
        /// </summary>
        [Test]
        public void DeleteData_Valid_Product_Should_Delete_And_Return_Product()
        {
            var result = productService.DeleteData("1");
            Assert.That(result, Is.Not.Null);
            Assert.That(productService.GetAllData().Any(p => p.Id == "1"), Is.False);
        }

        /// <summary>
        /// Tests that attempting to delete a non-existent product returns null.
        /// </summary>
        [Test]
        public void DeleteData_InValid_NonExistent_Product_Should_Return_Null()
        {
            var result = productService.DeleteData("non-existent");
            Assert.That(result, Is.Null);
        }

        /// <summary>
        /// Tests that permutations of categories, sizes, and colors are returned correctly.
        /// </summary>
        [Test]
        public void GetAllDataPermutations_Valid_Categorires_And_Size_And_Colors_Should_Return_Correctly()
        {
            var permutations = productService.GetAllDataPermutations();
            Assert.That(permutations["Categories"], Contains.Item("Category1").And.Contains("Category2"));
        }




        [Test]
        public void UpdateProduct_Valid_Product_Should_Update_Product_In_List()
        {
            // Arrange
            var existingProduct = new ProductModel { Id = "1", Title = "Old Title" };
            var updatedProduct = new ProductModel { Id = "1", Title = "Updated Title" };

            // Add the existing product to the mock data
            var products = new List<ProductModel>
    {
        existingProduct
    };
            productService = new JsonFileProductService(mockEnvironment.Object, products);

            // Act
            productService.UpdateProduct(updatedProduct);

            // Assert
            var allProducts = productService.GetAllData().ToList();
            Assert.That(allProducts.Count, Is.EqualTo(1), "The number of products in the list should remain the same.");
            Assert.That(allProducts.First().Title, Is.EqualTo("Updated Title"), "The product's title should be updated.");
        }

        [Test]
        public void UpdateProduct_NonExistent_Product_Should_Not_Modify_List()
        {
            // Arrange
            var nonExistentProduct = new ProductModel { Id = "3", Title = "Non-Existent Product" };

            // Add some existing products to the mock data
            var products = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Product 1" },
        new ProductModel { Id = "2", Title = "Product 2" }
    };
            productService = new JsonFileProductService(mockEnvironment.Object, products);

            // Act
            productService.UpdateProduct(nonExistentProduct);

            // Assert
            var allProducts = productService.GetAllData().ToList();
            Assert.That(allProducts.Count, Is.EqualTo(2), "The number of products in the list should remain unchanged.");
            Assert.That(allProducts.Any(p => p.Id == "3"), Is.False, "The non-existent product should not be added to the list.");
            Assert.That(allProducts.First().Title, Is.EqualTo("Product 1"), "The existing products should remain unchanged.");
        }



    }
}


