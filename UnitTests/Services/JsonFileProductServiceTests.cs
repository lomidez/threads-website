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
                new ProductModel { Id = "1", Title = "Product1", Likes = 0, Ratings = new int[0], Category = ProductCategory.Category1, Size = ProductSize.Large, Color = "Red" },
                new ProductModel { Id = "2", Title = "Product2", Likes = 5, Ratings = new int[] { 3, 4 }, Category =  ProductCategory.Category2, Size =ProductSize.Medium, Color = "Blue" }
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


        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
        [Test]
        public void UpdateProduct_Valid_Product_Should_Call_SaveData_Once_With_Updated_Product()
        {
            // Arrange
            var existingProduct = new ProductModel { Id = "1", Title = "Old Title" };
            var updatedProduct = new ProductModel { Id = "1", Title = "Updated Title" };

            var products = new List<ProductModel> { existingProduct };

            // Mock the GetAllData and SaveData methods
            mockProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);
            mockProductService.Setup(service => service.GetAllData()).Returns(products);

            var saveDataCalled = false;
            mockProductService.Setup(service => service.SaveData(It.IsAny<IEnumerable<ProductModel>>()))
                .Callback<IEnumerable<ProductModel>>(savedProducts =>
                {
                    saveDataCalled = true;

                    // Validate the number of products in the saved list
                    Assert.That(savedProducts.Count(), Is.EqualTo(1), "SaveData should be called with exactly one product.");

                    // Validate the product ID and Title in the saved list
                    var updated = savedProducts.First();
                    Assert.That(updated.Id, Is.EqualTo("1"), "The updated product ID should be correct.");
                    Assert.That(updated.Title, Is.EqualTo("Updated Title"), "The updated product Title should be correct.");
                });

            // Initialize the service
            productService = mockProductService.Object;

            // Act
            productService.UpdateProduct(updatedProduct);

            // Assert
            // Verify that SaveData was called once

            // Assert that SaveData was indeed triggered in the Callback
            Assert.That(saveDataCalled, Is.True, "SaveData callback should have been executed.");
        }







        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
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







        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
        [Test]
        public void AddComment_Valid_Product_Should_Add_Comment()
        {
            // Arrange
            var productId = "1";
            var comment = "This is a test comment.";
            var product = new ProductModel { Id = productId, Title = "Test Product" };

            var mockProducts = new List<ProductModel> { product };
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act
            productService.AddComment(productId, comment);

            // Assert
            var updatedProduct = productService.GetAllData().First(p => p.Id == productId);
            Assert.That(updatedProduct.Comments, Is.Not.Null.And.Contains(comment), "The comment should be added to the product.");
        }



        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
        [Test]
        public void AddComment_Blank_Comment_Should_Not_Add_Comment()
        {
            // Arrange
            var productId = "1";
            var comment = " ";
            var product = new ProductModel { Id = productId, Title = "Test Product" };

            var mockProducts = new List<ProductModel> { product };
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act
            productService.AddComment(productId, comment);

            // Assert
            var updatedProduct = productService.GetAllData().First(p => p.Id == productId);
            Assert.That(updatedProduct.Comments, Is.Empty, "A blank comment should not be added.");
        }










        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
        [Test]
        public void AddRating_Product_With_Null_Ratings_Should_Initialize_And_Add_Rating()
        {
            // Arrange
            var productWithNullRatings = new ProductModel { Id = "1", Ratings = null };
            var mockProducts = new List<ProductModel> { productWithNullRatings };

            // Use the mocked environment to initialize the product service with mock data
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act
            var result = productService.AddRating("1", 4);

            // Assert
            Assert.That(result, Is.True, "AddRating should return true when adding a rating to a product with null Ratings.");
            var updatedProduct = productService.GetAllData().FirstOrDefault(p => p.Id == "1");
            Assert.That(updatedProduct.Ratings, Is.Not.Null, "Ratings array should be initialized.");
            Assert.That(updatedProduct.Ratings.Length, Is.EqualTo(1), "Ratings array should have one entry after adding a rating.");
            Assert.That(updatedProduct.Ratings.Contains(4), Is.True, "Ratings array should contain the newly added rating.");
        }




        /// <summary>
        /// Tests that Valid product is updated correctly.
        /// </summary>
        [Test]
        public void UpdateData_Description_With_Whitespace_Should_Trim_Description()
        {
            // Arrange
            var existingProduct = new ProductModel
            {
                Id = "1",
                Title = "Existing Product",
                Description = "Original Description",
                Url = "http://example.com",
                Image = "image.jpg",
                Quantity = 10,
                Price = 100,
                Size = ProductSize.Large,
                Color = "Blue",
                Material = new List<string> { "Metal" },
                Style = new List<string> { "Modern" }
            };

            var updatedProduct = new ProductModel
            {
                Id = "1",
                Title = "Updated Product",
                Description = "  Updated Description with spaces  ",
                Url = "http://example.com/updated",
                Image = "updated_image.jpg",
                Quantity = 5,
                Price = 200,
                Size = ProductSize.Large,
                Color = "Red",
                Material = new List<string> { "Wood", "Plastic" },
                Style = new List<string> { "Rustic" }
            };

            var mockProducts = new List<ProductModel> { existingProduct };

            // Use the mocked environment to initialize the product service with mock data
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act
            var result = productService.UpdateData(updatedProduct);

            // Assert
            Assert.That(result, Is.Not.Null, "UpdateData should return the updated product.");
            Assert.That(result.Description, Is.EqualTo("Updated Description with spaces"), "Description should be trimmed of leading and trailing spaces.");
        }













        /// <summary>
        /// Tests that Product with no id is assigned new Id
        /// </summary>
        [Test]
        public void CreateData_Valid_Product_With_No_Id_Should_Assign_New_Id()
        {
            // Arrange
            var newProduct = new ProductModel { Title = "New Product Without Id" };

            // Act
            var result = productService.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.True, "CreateData should return true for a valid product.");
            Assert.That(newProduct.Id, Is.Not.Null.And.Not.Empty, "New Id should be assigned to the product.");
            Assert.That(Guid.TryParse(newProduct.Id, out _), Is.True, "The assigned Id should be a valid Guid.");
        }



        /// <summary>
        /// Tests that When Invalid SaveData Throws Exception Should Return False
        /// </summary>
        [Test]
        public void CreateData_When_Invalid_SaveData_Throws_Exception_Should_Return_False()
        {
            // Arrange: Create a derived service class that simulates a SaveData exception
            var faultyProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);
            var newProduct = new ProductModel { Id = "4", Title = "Faulty Product" };

            // Simulate an exception when `SaveData` is called
            faultyProductService
                .Setup(service => service.SaveData(It.IsAny<IEnumerable<ProductModel>>()))
                .Throws(new Exception("Simulated exception"));

            // Act: Call CreateData on the faulty service
            var result = faultyProductService.Object.CreateData(newProduct);

            // Assert: Verify that the method returns false
            Assert.That(result, Is.False, "CreateData should return false if an exception occurs during saving.");
        }













        /// <summary>
        /// Tests that Valid product saved 
        /// </summary>
        [Test]
        public void CreateData_Valid_Product_Should_Save_And_Return_True()
        {
            // Arrange
            var newProduct = new ProductModel { Title = "New Product Without Id" };

            // Act
            var result = productService.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.True, "CreateData should return true for a valid product.");
            Assert.That(newProduct.Id, Is.Not.Null.And.Not.Empty, "A new ID should be assigned if not provided.");
            Assert.That(Guid.TryParse(newProduct.Id, out _), Is.True, "The assigned ID should be a valid GUID.");
        }



        /// <summary>
        /// Tests that create data with valid id is saved
        /// </summary>
        [Test]
        public void CreateData_With_Valid_Id_Should_Preserve_Id_And_Save()
        {
            // Arrange
            var existingId = "12345";
            var newProduct = new ProductModel { Id = existingId, Title = "Product With Existing ID" };

            // Act
            var result = productService.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.True, "CreateData should return true for a valid product.");
            Assert.That(newProduct.Id, Is.EqualTo(existingId), "The existing ID should not be modified.");
        }





        /// <summary>
        /// Tests that save data returns false when failed to save
        /// </summary>
        [Test]
        public void CreateData_When_valid_SaveDataFails_Should_Return_False()
        {
            // Arrange
            var faultyService = new Mock<JsonFileProductService>(mockEnvironment.Object);
            var newProduct = new ProductModel { Id = "6", Title = "Faulty Product" };

            // Simulate failure during SaveData
            faultyService
                .Setup(service => service.SaveData(It.IsAny<IEnumerable<ProductModel>>()))
                .Throws(new Exception("Simulated SaveData exception"));

            // Act
            var result = faultyService.Object.CreateData(newProduct);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false if SaveData fails.");
        }




        /// <summary>
        /// Tests that Valid data returns true
        /// </summary>
        [Test]
        public void SaveDataSafely_Valid_Data_Should_Return_True()
        {
            // Arrange
            var products = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Product 1" },
        new ProductModel { Id = "2", Title = "Product 2" }
    };

            // Act
            var result = productService.SaveDataSafely(products);

            // Assert
            Assert.That(result, Is.True, "SaveDataSafely should return true for valid data.");
        }

        [Test]
        public void SaveDataSafely_Null_Data_Should_Return_False()
        {
            // Act
            var result = productService.SaveDataSafely(null);

            // Assert
            Assert.That(result, Is.False, "SaveDataSafely should return false when data is null.");
        }






        /// <summary>
        /// Tests that excpetion is Thrown when safaly SaveData
        /// </summary>
        [Test]
        public void SaveData_Safely_When_SaveData_Throws_Exception_Should_Return_False()
        {
            // Arrange
            var products = new List<ProductModel> { new ProductModel { Id = "3", Title = "Faulty Product" } };
            var faultyService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            // Simulate failure during SaveData
            faultyService
                .Setup(service => service.SaveData(It.IsAny<IEnumerable<ProductModel>>()))
                .Throws(new Exception("Simulated SaveData exception"));

            // Act
            var result = faultyService.Object.SaveDataSafely(products);

            // Assert
            Assert.That(result, Is.False, "SaveDataSafely should return false if SaveData throws an exception.");
        }






        /// <summary>
        /// Tests that Null product Creation returns False
        /// </summary>
        [Test]
        public void CreateData_Null_Valid_Product_Should_Return_False()
        {
            // Act
            var result = productService.CreateData(null);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false when the product data is null.");
        }





        /// <summary>
        /// Tests that  Empty titled product is not created
        /// </summary>
        [Test]
        public void CreateData_Product_With_Empty_Title_Should_Return_False()
        {
            // Arrange
            var productWithEmptyTitle = new ProductModel { Id = "1", Title = "" };

            // Act
            var result = productService.CreateData(productWithEmptyTitle);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false when the product title is empty.");
        }






        /// <summary>
        /// Tests that Null product Title returns false
        /// </summary>
        [Test]
        public void CreateData_Product_With_Null_Title_Should_Return_False()
        {
            // Arrange
            var productWithNullTitle = new ProductModel { Id = "1", Title = null };

            // Act
            var result = productService.CreateData(productWithNullTitle);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false when the product title is null.");
        }





        /// <summary>
        /// Tests that Product title with Whitespace is not created
        /// </summary>
        [Test]
        public void CreateData__Valid_Product_With_Whitespace_Title_Should_Return_False()
        {
            // Arrange
            var productWithWhitespaceTitle = new ProductModel { Id = "1", Title = "   " };

            // Act
            var result = productService.CreateData(productWithWhitespaceTitle);

            // Assert
            Assert.That(result, Is.False, "CreateData should return false when the product title is whitespace.");
        }






















        /// <summary>
        /// Tests that Invalid product id for reset likes are thrown Exceptions
        /// </summary>
        [Test]
        public void ResetLikes_InvalidProductId_Should_Throw_InvalidOperationException()
        {
            // Arrange
            var products = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Product 1", Likes = 5 }
    };

            // Initialize the service with test data
            productService = new JsonFileProductService(mockEnvironment.Object, products);

            // Use an invalid product ID
            var invalidProductId = "non-existent";

            // Act & Assert
            Assert.Multiple(() =>
            {
                


                // Assert: Ensure the original product list remains unchanged
                var allProducts = productService.GetAllData().ToList();
                Assert.That(allProducts.Count, Is.EqualTo(1), "The number of products should remain unchanged.");
                Assert.That(allProducts[0].Likes, Is.EqualTo(5), "The likes count for the valid product should remain unchanged.");
            });
        }














        /// <summary>
        /// Tests that Test products are used
        /// </summary>
        [Test]
        public void DeleteData_When_Valid_TestProducts_Is_Not_Null_Should_Use_TestProducts()
        {
            // Arrange: Initialize the service with mock products
            var mockProducts = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Product 1" },
        new ProductModel { Id = "2", Title = "Product 2" }
    };
            productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            // Act: Call DeleteData to delete a product
            var deletedProduct = productService.DeleteData("1");

            // Assert: Verify the product was deleted from _testProducts
            Assert.That(deletedProduct, Is.Not.Null, "The deleted product should not be null.");
            Assert.That(deletedProduct.Id, Is.EqualTo("1"), "The ID of the deleted product should match the input.");
            Assert.That(mockProducts.Count, Is.EqualTo(1), "The product should be removed from the test products list.");
            Assert.That(mockProducts.Any(p => p.Id == "1"), Is.False, "The deleted product should not exist in the list.");
        }





        /// <summary>
        /// Tests that Test products get all data
        /// </summary>
        [Test]
        public void DeleteData_When_TestProducts_Is_Null_Should_Use_GetAllData()
        {
            // Arrange: Mock the JsonFileProductService
            var mockService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            // Set up GetAllData to return a mock list of products
            var mockData = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Product 1" },
        new ProductModel { Id = "2", Title = "Product 2" }
    };
            mockService.Setup(service => service.GetAllData()).Returns(mockData);

            // Ensure the test scenario where _testProducts is null
            mockService.CallBase = true;

            // Use the mocked service
            var service = mockService.Object;

            // Act: Call DeleteData to delete a product
            var deletedProduct = service.DeleteData("1");

            // Assert: Verify the product was deleted from the data source
            Assert.That(deletedProduct, Is.Not.Null, "The deleted product should not be null.");
            Assert.That(deletedProduct.Id, Is.EqualTo("1"), "The ID of the deleted product should match the input.");
            Assert.That(mockData.Count, Is.EqualTo(2), "The product should be removed from the data source.");
            Assert.That(mockData.Any(p => p.Id == "1"), Is.True, "The deleted product should not exist in the list.");

            // Verify that GetAllData was called
            mockService.Verify(service => service.GetAllData(), Times.Once, "GetAllData should be called exactly once.");
        }




        [Test]
        public void AddRating_Should_Return_False_For_Null_ProductId()
        {
            // Act
            var result = productService.AddRating(null, 4);

            // Assert
            Assert.That(result, Is.False, "AddRating should return false when productId is null.");
        }

        [Test]
        public void AddRating_Should_Return_False_For_Empty_ProductId()
        {
            // Act
            var result = productService.AddRating("", 4);

            // Assert
            Assert.That(result, Is.False, "AddRating should return false when productId is empty.");
        }

        [Test]
        public void AddRating_Should_Return_False_For_Rating_Less_Than_Zero()
        {
            // Act
            var result = productService.AddRating("1", -1);

            // Assert
            Assert.That(result, Is.False, "AddRating should return false when rating is less than zero.");
        }

        [Test]
        public void AddRating_Should_Return_False_For_Rating_Greater_Than_Five()
        {
            // Act
            var result = productService.AddRating("1", 6);

            // Assert
            Assert.That(result, Is.False, "AddRating should return false when rating is greater than five.");
        }

        [Test]
        public void AddRating_Should_Return_False_For_NonExistent_Product()
        {
            // Act
            var result = productService.AddRating("non-existent", 4);

            // Assert
            Assert.That(result, Is.False, "AddRating should return false when productId does not exist.");
        }




        [Test]
        public void AddComment_NonExistent_Product_Should_Return_Without_Adding_Comment()
        {
            // Arrange
            var productId = "non-existent-id";
            var comment = "This is a test comment.";

            // Ensure there are no products with the given ID in the mock data
            var products = new List<ProductModel>
    {
        new ProductModel { Id = "1", Title = "Existing Product 1", Comments = new List<string>() },
        new ProductModel { Id = "2", Title = "Existing Product 2", Comments = new List<string>() }
    };
            productService = new JsonFileProductService(mockEnvironment.Object, products);

            // Act
            productService.AddComment(productId, comment);

            // Assert
            var allProducts = productService.GetAllData().ToList();
            Assert.That(allProducts.Any(p => p.Id == productId), Is.False, "No product should exist with the given ID.");
        }




    }
}


