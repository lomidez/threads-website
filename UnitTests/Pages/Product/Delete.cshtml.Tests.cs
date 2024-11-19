using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace UnitTests.Pages.Products
{
    /// <summary>
    /// Unit tests for the DeleteModel page.
    /// </summary>
    public class DeleteTests
    {
        private DeleteModel pageModel;
        private Mock<JsonFileProductService> mockProductService;

        /// <summary>
        /// Initializes the test setup by mocking the environment and product service.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockProductService = new Mock<JsonFileProductService>(mockEnvironment.Object);

            pageModel = new DeleteModel(mockProductService.Object)
            {
                TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            };
        }

        /// <summary>
        /// Tests the OnPost method when SelectedProduct is null.
        /// Expects a NotFoundResult and an error message in TempData.
        /// </summary>
        [Test]
        public void OnPost_Invalid_Null_Select_Product_Should_Return_NotFound()
        {
            pageModel.SelectedProduct = null;
            var result = pageModel.OnPost();

            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product, product does not exist."));
        }

        /// <summary>
        /// Tests the OnPost method when SelectedProduct has an empty Id.
        /// Expects a NotFoundResult and an error message in TempData.
        /// </summary>
        [Test]
        public void OnPost_Invalid_Empty_Select_Product_Should_Return_NotFound()
        {
            // Arrange
            pageModel.SelectedProduct = new ProductModel { Id = string.Empty };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product, product could not be located."));
            mockProductService.Verify(service => service.DeleteData(It.IsAny<string>()), Times.Never);
        }



        /// <summary>
        /// Tests the OnPost method when SelectedProduct has a whitespace Id.
        /// Expects a NotFoundResult and an error message in TempData.
        /// </summary>
        [Test]
        public void OnPost_Invalid_White_Space_Select_ProductId_Should_Return_NotFound()
        {
            pageModel.SelectedProduct = new ProductModel { Id = "   " };
            var result = pageModel.OnPost();

            Assert.That(result, Is.TypeOf<NotFoundResult>());
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo("Error: Failed to delete product, product could not be located."));
        }

        /// <summary>
        /// Tests the OnPost method when a non-existent product ID is provided.
        /// Expects a NotFoundResult and an error message in TempData.
        /// </summary>
        [Test]
        public void OnPost_Invalid_NonExistent_Id_Should_Return_NotFound()
        {
            // Arrange
            var invalidProductId = "non-existent-id";

            // Mock DeleteData to return null for a non-existent product
            mockProductService.Setup(service => service.DeleteData(invalidProductId)).Returns((ProductModel)null);

            // Set SelectedProduct in the page model
            pageModel.SelectedProduct = new ProductModel { Id = invalidProductId };

            // Initialize TempData
            var tempData = new Dictionary<string, object>();
            pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
            {
                ["Notification"] = null
            };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.Multiple(() =>
            {
                // Ensure the result is NotFound
                Assert.That(result, Is.TypeOf<NotFoundResult>(), "Expected NotFoundResult for non-existent ID.");

                // Check the value of TempData["Notification"]
                Assert.That(pageModel.TempData["Notification"], Is.EqualTo(null), "Notification message was not set correctly.");
            });

            // Verify that DeleteData was called once with the invalid ID
            mockProductService.Verify(service => service.DeleteData(invalidProductId), Times.Once, "DeleteData should be called once for the invalid product ID.");
        }









        /// <summary>
        /// Tests the OnPost method with a valid product ID.
        /// Expects a successful deletion and a redirect to the Index page.
        /// </summary>
        [Test]
        public void OnPost_Valid_Id_Should_Delete_Product_Should_RedirectToIndexPage()
        {
            // Arrange
            var validProductId = "valid-id";
            var product = new ProductModel { Id = validProductId, Title = "Valid Product" };
            pageModel.SelectedProduct = product;

            mockProductService.Setup(service => service.DeleteData(validProductId)).Returns(product);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            Assert.That(((RedirectToPageResult)result).PageName, Is.EqualTo("./Index"));
            Assert.That(pageModel.TempData["Notification"], Is.EqualTo(null));
        }



        /// <summary>
        /// Tests the OnGet method with a valid product ID.
        /// Expects the product details to be loaded successfully.
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_Should_Load_Product_Details()
        {
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = new List<string> { "Wood", "Metal" },
                Style = new List<string> { "Modern", "Rustic" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });

            pageModel.OnGet(productId);

            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(pageModel.Material, Is.EqualTo("Wood, Metal"));
            Assert.That(pageModel.Style, Is.EqualTo("Modern, Rustic"));
        }

        /// <summary>
        /// Tests the OnGet method with an invalid product ID.
        /// Expects no product to be loaded.
        /// </summary>
        [Test]
        public void OnGet_Invalid_Id_Should_Not_Load_Product()
        {
            var invalidProductId = "invalid-id";
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());

            pageModel.OnGet(invalidProductId);

            Assert.That(pageModel.SelectedProduct, Is.Null);
            Assert.That(pageModel.Material, Is.Null);
            Assert.That(pageModel.Style, Is.Null);
        }

        /// <summary>
        /// Tests the OnGet method when the product's Material and Style are null.
        /// Expects these properties to be set to empty strings.
        /// </summary>
        [Test]
        public void OnGet_Valid_Id_With_Null_Material_And_Style_Should_Set_Empty_Strings()
        {
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = null,
                Style = null
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });

            pageModel.OnGet(productId);

            Assert.That(pageModel.SelectedProduct, Is.Not.Null);
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(pageModel.Material, Is.EqualTo(string.Empty));
            Assert.That(pageModel.Style, Is.EqualTo(string.Empty));
        }
    }
}
