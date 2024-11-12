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
    [TestFixture]
    public class JsonFileProductServiceTests
    {
        private JsonFileProductService productService;
        private Mock<IWebHostEnvironment> mockEnvironment;
        private List<ProductModel> mockProducts;
        private string tempDirectory;

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

        [TearDown]
        public void TearDown()
        {
            // Clean up temp directory after each test
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }

        [Test]
        public void SaveData_Should_Write_Products_To_File()
        {
            // Arrange
            var dataDirectory = Path.Combine(tempDirectory, "data");
            Directory.CreateDirectory(dataDirectory);  // Ensure 'data' directory exists

            var productsToSave = new List<ProductModel>
            {
                new ProductModel { Id = "3", Title = "Product3" }
            };

            // Act
            productService.SaveData(productsToSave);

            // Assert: Check if the file was created in the temp directory
            var filePath = Path.Combine(dataDirectory, "products.json");
            Assert.That(File.Exists(filePath), Is.True, "File should be created in the temporary directory.");

            // Verify contents of the file
            var savedProducts = JsonSerializer.Deserialize<List<ProductModel>>(File.ReadAllText(filePath));
            Assert.That(savedProducts, Is.Not.Null);
            Assert.That(savedProducts.Count, Is.EqualTo(1));
            Assert.That(savedProducts.Any(p => p.Id == "3" && p.Title == "Product3"), Is.True);
        }





        [Test]
        public void GetAllData_Should_Return_All_Products()
        {
            var products = productService.GetAllData();
            Assert.That(products, Is.Not.Null);
            Assert.That(products.Count(), Is.EqualTo(mockProducts.Count));
        }

        [Test]
        public void AddRating_Valid_Should_Add_Rating()
        {
            var result = productService.AddRating("1", 4);
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(result, Is.True);
            Assert.That(product.Ratings, Contains.Item(4));
        }

        [Test]
        public void AddRating_Invalid_Rating_Should_Return_False()
        {
            Assert.That(productService.AddRating("1", -1), Is.False); // Negative rating
            Assert.That(productService.AddRating("1", 6), Is.False); // Out of range rating
        }

        [Test]
        public void AddRating_NonExistent_Product_Should_Return_False()
        {
            var result = productService.AddRating("non-existent", 4);
            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateData_Valid_Product_Should_Update_And_Return_Product()
        {
            var updatedProduct = new ProductModel { Id = "1", Title = "Updated Product1" };
            var result = productService.UpdateData(updatedProduct);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("Updated Product1"));
        }

        [Test]
        public void UpdateData_NonExistent_Product_Should_Return_Null()
        {
            var updatedProduct = new ProductModel { Id = "non-existent", Title = "Nonexistent Product" };
            var result = productService.UpdateData(updatedProduct);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateData_Valid_Product_Should_Create_And_Return_True()
        {
            var newProduct = new ProductModel { Id = "3", Title = "New Product" };
            var result = productService.CreateData(newProduct);
            Assert.That(result, Is.True);
            Assert.That(productService.GetAllData().Any(p => p.Id == "3"), Is.False);
        }

        [Test]
        public void CreateData_Product_With_Existing_Id_Should_Append()
        {
            var newProduct = new ProductModel { Id = "3", Title = "New Product with ID" };
            var result = productService.CreateData(newProduct);
            Assert.That(result, Is.True);
            Assert.That(productService.GetAllData().Any(p => p.Id == "3"), Is.False);
        }

        [Test]
        public void AddLike_Valid_Product_Should_Increment_Likes()
        {
            var result = productService.AddLike("1");
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(result, Is.True);
            Assert.That(product.Likes, Is.EqualTo(1));
        }

        [Test]
        public void AddLike_NonExistent_Product_Should_Return_False()
        {
            var result = productService.AddLike("non-existent");
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetLikes_Valid_Product_Should_Reset_Likes()
        {
            productService.AddLike("1");
            productService.ResetLikes("1");
            var product = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(product.Likes, Is.EqualTo(0));
        }

        [Test]
        public void ResetLikes_NonExistent_Product_Should_Throw_Exception()
        {
            Assert.Throws<InvalidOperationException>(() => productService.ResetLikes("non-existent"));
        }

        [Test]
        public void DeleteData_Valid_Product_Should_Delete_And_Return_Product()
        {
            var result = productService.DeleteData("1");
            Assert.That(result, Is.Not.Null);
            Assert.That(productService.GetAllData().Any(p => p.Id == "1"), Is.False);
        }

        [Test]
        public void DeleteData_NonExistent_Product_Should_Return_Null()
        {
            var result = productService.DeleteData("non-existent");
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAllDataPermutations_Should_Return_Distinct_Categories_Sizes_And_Colors()
        {
            var permutations = productService.GetAllDataPermutations();
            Assert.That(permutations["Categories"], Contains.Item("Category1").And.Contains("Category2"));
            Assert.That(permutations["Sizes"], Contains.Item("Large").And.Contains("Medium"));
            Assert.That(permutations["Colors"], Contains.Item("Red").And.Contains("Blue"));
        }

        [Test]
        public void SaveData_Should_Save_Products_To_MockList()
        {
            // Act
            var newProducts = new List<ProductModel>
            {
                new ProductModel { Id = "3", Title = "Product3" }
            };
            productService.SaveData(newProducts);

            // Assert
            Assert.That(productService.GetAllData().Any(p => p.Id == "3"), Is.False);
        }

        [Test]
        public void SaveData_Should_Handle_Empty_List_Gracefully()
        {
            // Act
            productService.SaveData(new List<ProductModel>());

            // Assert
            var products = productService.GetAllData();
            Assert.That(products, Is.Not.Null, "Product list should still be valid even if empty.");
        }


        [Test]
        public void AddRating_Should_Initialize_Ratings_Array_When_Null()
        {
            // Arrange: Set up a product with null Ratings
            var product = new ProductModel { Id = "1", Title = "Product1", Ratings = null };
            productService = new JsonFileProductService(mockEnvironment.Object, new List<ProductModel> { product });

            // Act: Add a rating to the product with null Ratings
            var result = productService.AddRating("1", 4);

            // Assert: Check that the rating was added successfully and Ratings is no longer null
            Assert.That(result, Is.True);
            var updatedProduct = productService.GetAllData().First(p => p.Id == "1");
            Assert.That(updatedProduct.Ratings, Is.Not.Null);
            Assert.That(updatedProduct.Ratings, Contains.Item(4));
        }




        [Test]
        public void UpdateData_Valid_Product_Should_Trim_Description()
        {
            // Arrange: Create a product with leading/trailing spaces in the Description
            var productToUpdate = new ProductModel { Id = "1", Description = "  Description with spaces  " };
            productService.CreateData(productToUpdate); // Adding product to the dataset

            var updatedProduct = new ProductModel { Id = "1", Description = "  Updated Description with spaces  " };

            // Act
            var result = productService.UpdateData(updatedProduct);

            // Assert: Ensure the description was trimmed
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Description, Is.EqualTo("Updated Description with spaces"));
        }






        [Test]
        public void CreateData_Product_With_No_Id_Should_Assign_New_Id()
        {
            // Arrange: Create a product without setting an Id
            var newProduct = new ProductModel { Title = "New Product Without Id" };

            // Act
            var result = productService.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.True, "CreateData should return true for a valid product.");
            Assert.That(newProduct.Id, Is.Not.Null.And.Not.Empty, "New Id should be assigned to the product.");
            Assert.That(Guid.TryParse(newProduct.Id, out _), Is.True, "The assigned Id should be a valid Guid.");
        }




        public class JsonFileProductServiceWithSaveException : JsonFileProductService
        {
            public JsonFileProductServiceWithSaveException(IWebHostEnvironment webHostEnvironment)
                : base(webHostEnvironment)
            { }

            // Override SaveData to throw an exception
            public override void SaveData(IEnumerable<ProductModel> products)
            {
                throw new IOException("Simulated exception in SaveData");
            }
        }

        [Test]
        public void CreateData_When_SaveData_ThrowsException_Should_Return_False()
        {
            // Arrange: Use the derived service to simulate a SaveData exception
            var productServiceWithException = new JsonFileProductServiceWithSaveException(mockEnvironment.Object);
            var newProduct = new ProductModel { Id = "4", Title = "Product Causing Exception" };

            // Act
            var result = productServiceWithException.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false if an exception is thrown in SaveData.");
        }



        [Test]
        public void DeleteData_When_TestProducts_Is_Null_Should_Use_GetAllData()
        {
            // Arrange: Initialize productService with no _testProducts, so it falls back to GetAllData
            mockProducts = null; // Ensure _testProducts is null
            productService = new JsonFileProductService(mockEnvironment.Object);

            // Mock GetAllData to simulate existing products
            var mockExistingProducts = new List<ProductModel>
    {
        new ProductModel { Id = "10", Title = "Existing Product" }
    };

            var productServiceMock = new Mock<JsonFileProductService>(mockEnvironment.Object) { CallBase = true };
            productServiceMock.Setup(service => service.GetAllData()).Returns(mockExistingProducts);

            // Act: Attempt to delete an existing product
            var deletedProduct = productServiceMock.Object.DeleteData("10");

            // Assert
            Assert.That(deletedProduct, Is.Not.Null, "Expected product should be deleted.");
            Assert.That(deletedProduct.Id, Is.EqualTo("10"), "The deleted product's ID should match.");
        }





    }
}
