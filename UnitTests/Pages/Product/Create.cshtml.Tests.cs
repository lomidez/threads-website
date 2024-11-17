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
    /// <summary>
    /// Unit tests for the <see cref="CreateModel"/> class.
    /// </summary>
    [TestFixture]
    public class CreateTests
    {
        /// <summary>
        /// Instance of the CreateModel being tested.
        /// </summary>
        private CreateModel pageModel;

        /// <summary>
        /// Mock service for testing interactions with JsonFileProductService.
        /// </summary>
        private Mock<JsonFileProductService> mockProductService;

        /// <summary>
        /// Mock list of products used for testing.
        /// </summary>
        private List<ProductModel> mockProducts;

        /// <summary>
        /// Initializes the test environment before each test case.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Mock product list setup
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "existing-id", Title = "Existing Product" }
            };

            // Mock service setup with IWebHostEnvironment
            mockProductService = new Mock<JsonFileProductService>(new Mock<IWebHostEnvironment>().Object);
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);

            // Initialize the CreateModel page model
            pageModel = new CreateModel(mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };

            // Clear any ModelState errors
            pageModel.ModelState.Clear();
        }

        /// <summary>
        /// Tests that the OnPost method returns a PageResult when the ModelState is invalid.
        /// </summary>
        [Test]
        public void OnPost_Invalid_ModelState_Should_Return_Page()
        {
            // Arrange: Add a ModelState error
            pageModel.ModelState.AddModelError("TestError", "Invalid model state");

            // Act: Call the OnPost method
            var result = pageModel.OnPost();

            // Assert: Verify that the result is of type PageResult
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        /// <summary>
        /// Tests that OnPost returns a PageResult with an error when product creation fails.
        /// </summary>
        [Test]
        public void OnPost_Valid_Create_Product_Fails_Should_Return_Page_With_Error()
        {
            // Arrange: Set up NewProduct and mock CreateData to fail
            pageModel.NewProduct = new ProductModel { Id = "new-id", Title = "New Product" };
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(false);

            // Act: Call the OnPost method
            var result = pageModel.OnPost();

            // Assert: Verify the result and ModelState error count
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState[string.Empty].Errors.Count, Is.GreaterThan(0));
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Once);
        }

        /// <summary>
        /// Tests that OnPost redirects to the Index page when product creation succeeds.
        /// </summary>
        [Test]
        public void OnPost_Valid_Create_Product_Success_Should_Redirect_To_Index()
        {
            // Arrange: Initialize a valid product and set up mock service
            pageModel.NewProduct = new ProductModel
            {
                Id = "new-id",
                Title = "New Product",
                Price = 10,
                Quantity = 5,
                Description = "Test Description"
            };

            pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            pageModel.ModelState.Clear();
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(true);

            // Act: Call the OnPost method
            var result = pageModel.OnPost();

            // Assert: Verify redirection and notification message
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("./Index"));
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Product successfully created."));
        }

        /// <summary>
        /// Tests that GenerateUniqueId method returns a valid GUID.
        /// </summary>
        [Test]
        public void GenerateUniqueId_Should_Return_Unique_Id()
        {
            // Act: Call the private GenerateUniqueId method
            var uniqueId = InvokeGenerateUniqueId();

            // Assert: Verify the uniqueId is a valid GUID
            Assert.That(uniqueId, Is.Not.Null.And.Not.Empty);
            Assert.That(Guid.TryParse(uniqueId, out _), Is.True, "UniqueId should be a valid GUID format.");
        }

        /// <summary>
        /// Tests that ValidateProductId returns false for null, empty, or whitespace IDs.
        /// </summary>
        [Test]
        public void ValidateProductId_NullOrWhitespace_Should_Return_False()
        {
            // Act: Call ValidateProductId with various inputs
            var resultNull = InvokeValidateProductId(null);
            var resultEmpty = InvokeValidateProductId("");
            var resultWhitespace = InvokeValidateProductId("   ");

            // Assert: Verify all results are false and ModelState errors are counted
            Assert.That(resultNull, Is.False);
            Assert.That(resultEmpty, Is.False);
            Assert.That(resultWhitespace, Is.False);
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests that ValidateProductId returns false for an invalid ID format.
        /// </summary>
        [Test]
        public void ValidateProductId_InvalidFormat_Should_Return_False()
        {
            // Act: Call ValidateProductId with an invalid format
            var result = InvokeValidateProductId("Invalid@ID!");

            // Assert: Verify result is false and ModelState has an error
            Assert.That(result, Is.False);
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Helper method to invoke the private GenerateUniqueId method via reflection.
        /// </summary>
        private string InvokeGenerateUniqueId()
        {
            var method = typeof(CreateModel).GetMethod("GenerateUniqueId", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)method.Invoke(pageModel, null);
        }

        /// <summary>
        /// Helper method to invoke the private ValidateProductId method via reflection.
        /// </summary>
        private bool InvokeValidateProductId(string productId)
        {
            var method = typeof(CreateModel).GetMethod("ValidateProductId", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(pageModel, new object[] { productId });
        }

        /// <summary>
        /// Helper method to invoke the private IsDuplicateProductId method via reflection.
        /// </summary>
        private bool InvokeIsDuplicateProductId(string productId)
        {
            var method = typeof(CreateModel).GetMethod("IsDuplicateProductId", BindingFlags.NonPublic | BindingFlags.Instance);
            return (bool)method.Invoke(pageModel, new object[] { productId });
        }
    }
}
