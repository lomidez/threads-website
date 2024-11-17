using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;

namespace UnitTests.Pages.Products
{
    [TestFixture]
    public class CreateTests
    {
        private CreateModel pageModel;
        private Mock<JsonFileProductService> mockProductService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void TestInitialize()
        {
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "existing-id", Title = "Existing Product" }
            };

            mockProductService = new Mock<JsonFileProductService>(new Mock<IWebHostEnvironment>().Object);
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);

            pageModel = new CreateModel(mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            pageModel.ModelState.Clear();
        }

        [Test]
        public void OnPost_Invalid_ModelState_Should_Return_Page()
        {
            // Arrange
            pageModel.ModelState.AddModelError("TestError", "Invalid model state");

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        [Test]
        public void OnPost_Valid_Create_Product_Fails_Should_Return_Page_With_Error()
        {
            // Arrange
            pageModel.NewProduct = new ProductModel { Id = "new-id", Title = "New Product" };
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(false);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState[string.Empty].Errors.Count, Is.GreaterThan(0));
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Once);
        }

        [Test]
        public void OnPost_Valid_Create_Product_Success_Should_Redirect_To_Index()
        {
            // Arrange: Set up a fully initialized and valid product
            pageModel.NewProduct = new ProductModel
            {
                Id = "new-id",
                Title = "New Product",
                Price = 10,
                Quantity = 5,
                Description = "Test Description"
            };

            // Initialize TempData and clear ModelState to ensure there are no pre-existing errors
            pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            pageModel.ModelState.Clear();

            // Mock CreateData to return true to simulate successful data creation
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(true);

            // Act: Call the OnPost method
            var result = pageModel.OnPost();

            // Assert: Check if redirection happens and if CreateData was called once
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("./Index"));
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Product successfully created."));
            //mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Once);
        }


        [Test]
        public void GenerateUniqueId_Should_Return_Unique_Id()
        {
            // Act
            var uniqueId = InvokeGenerateUniqueId();

            // Assert
            Assert.That(uniqueId, Is.Not.Null.And.Not.Empty);
            Assert.That(Guid.TryParse(uniqueId, out _), Is.True, "UniqueId should be a valid GUID format.");
        }

        [Test]
        public void ValidateProductId_NullOrWhitespace_Should_Return_False()
        {
            // Act
            var resultNull = InvokeValidateProductId(null);
            var resultEmpty = InvokeValidateProductId("");
            var resultWhitespace = InvokeValidateProductId("   ");

            // Assert
            Assert.That(resultNull, Is.False);
            Assert.That(resultEmpty, Is.False);
            Assert.That(resultWhitespace, Is.False);
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.EqualTo(3));
        }

        [Test]
        public void ValidateProductId_InvalidFormat_Should_Return_False()
        {
            // Act
            var result = InvokeValidateProductId("Invalid@ID!");

            // Assert
            Assert.That(result, Is.False);
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void OnPost_ValidProductId_Should_Pass_Validation()
        {
            // Arrange: Use a valid product ID
            pageModel.NewProduct = new ProductModel
            {
                Id = "Valid-123",
                Title = "Sample Product",
                Price = 50
            };

            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(true);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
        }

        [Test]
        public void ValidateProductId_ValidFormat_Should_Return_True()
        {
            // Arrange: Use a valid product ID that meets all criteria
            var validProductId = "Valid-123";

            // Clear ModelState to avoid pre-existing errors from other tests
            pageModel.ModelState.Clear();

            // Act: Call ValidateProductId using reflection
            var result = InvokeValidateProductId(validProductId);

            // Assert: Ensure that validation passes
            Assert.That(result, Is.True, "Expected ValidateProductId to return true for a valid ID format.");

            // Check if "NewProduct.Id" exists in ModelState; if it does, ensure no errors were added
            if (pageModel.ModelState.ContainsKey("NewProduct.Id"))
            {
                Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.EqualTo(0), "Expected no ModelState errors for a valid ID.");
            }
            else
            {
                Assert.Pass("No ModelState entry for 'NewProduct.Id' implies no errors.");
            }
        }


        [Test]
        public void IsDuplicateProductId_ExistingId_Should_Return_True()
        {
            // Act
            var result = InvokeIsDuplicateProductId("existing-id");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsDuplicateProductId_NonExistingId_Should_Return_False()
        {
            // Act
            var result = InvokeIsDuplicateProductId("new-id");

            // Assert
            Assert.That(result, Is.False);
        }

        // Helper methods to access private methods via reflection
        private string InvokeGenerateUniqueId()
        {
            var method = typeof(CreateModel).GetMethod("GenerateUniqueId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (string)method.Invoke(pageModel, null);
        }

        private bool InvokeValidateProductId(string productId)
        {
            // Use reflection to get the private method ValidateProductId from CreateModel
            var method = typeof(CreateModel).GetMethod("ValidateProductId", BindingFlags.NonPublic | BindingFlags.Instance);

            // Verify that the method was retrieved successfully
            if (method == null)
            {
                throw new InvalidOperationException("ValidateProductId method not found in CreateModel.");
            }

            return (bool)method.Invoke(pageModel, new object[] { productId });
        }

        private bool InvokeIsDuplicateProductId(string productId)
        {
            var method = typeof(CreateModel).GetMethod("IsDuplicateProductId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (bool)method.Invoke(pageModel, new object[] { productId });
        }
    }
}
