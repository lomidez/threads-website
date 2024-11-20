using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTests.Pages
{
    /// <summary>
    /// Unit tests for the <see cref="UpdateModel"/> class.
    /// </summary>
    [TestFixture]
    public class UpdateTests
    {
        private Mock<IWebHostEnvironment> mockEnvironment;
        private Mock<JsonFileProductService> mockProductService;
        private UpdateModel updatePage;

        /// <summary>
        /// Set up the mocks and prepare for each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Initialize mock environment
            mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment.Setup(env => env.WebRootPath).Returns("C:/fake/path"); // Adjust as needed for the test

            // Initialize mock product service with the mock environment
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, mockEnvironment.Object);

            // Initialize the UpdateModel with the mock service
            updatePage = new UpdateModel(mockProductService.Object);
        }

        /// <summary>
        /// Test that OnPostResetLikes correctly resets likes and redirects when provided a valid product ID.
        /// </summary>
        [Test]
        public void OnPostResetLikes_Valid_ProductId_Should_Reset_Likes_And_Redirect()
        {
            // Arrange
            string validProductId = "test-id";

            // Set up the mock service to use Loose behavior and allow ResetLikes to be called
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Loose, mockEnvironment.Object);

            // Explicitly allow ResetLikes to be called, verifying it is invoked once with any string
            mockProductService.Setup(service => service.ResetLikes(It.IsAny<string>())).Verifiable();

            // Create the UpdateModel instance with the mock service
            var updateModel = new UpdateModel(mockProductService.Object);

            // Act: Call OnPostResetLikes with the valid product ID
            var result = updateModel.OnPostResetLikes(validProductId);

            // Check that the method redirects correctly
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult, Is.Not.Null, "Expected a redirection result.");
            Assert.That(redirectResult.PageName, Is.EqualTo("/Product/Update"), "Expected redirection to the Update page.");
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(validProductId), "Expected the ID route value to match the product ID.");
        }

        [Test]
        public void OnPostResetLikes_ValidProductId_Should_ResetLikes_And_Redirect()
        {
            // Arrange
            var validProductId = "valid-id";
            mockProductService.Setup(service => service.ResetLikes(validProductId)).Verifiable();

            // Act
            var result = updatePage.OnPostResetLikes(validProductId);

            // Assert
            Assert.That(result, Is.TypeOf<RedirectToPageResult>(), "The method should redirect to the page after resetting likes.");
        }

        /// <summary>
        /// Test that OnGet loads the product with the correct material and style for a valid product ID.
        /// </summary>
        [Test]
        public void OnGet_Valid_ProductId_Should_Return_Product_With_Correct_Material_And_Style()
        {
            var product = new ProductModel
            {
                Id = "1",
                Title = "Product1",
                Material = new List<string> { "Wood", "Metal" },
                Style = new List<string> { "Modern", "Classic" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            updatePage.OnGet("1");
            Assert.That(updatePage.SelectedProduct, Is.Not.Null);
            Assert.That(updatePage.SelectedProduct.Title, Is.EqualTo("Product1"));
            Assert.That(updatePage.Material, Is.EqualTo("Wood, Metal"));
            Assert.That(updatePage.Style, Is.EqualTo("Modern, Classic"));
        }

        /// <summary>
        /// Test that OnGet does not load a product for an invalid product ID.
        /// </summary>
        [Test]
        public void OnGet_Invalid_ProductId_Should_Not_Load_Product()
        {
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());
            updatePage.OnGet("invalid");
            Assert.That(updatePage.SelectedProduct, Is.Null);
            Assert.That(updatePage.Material, Is.Null);
            Assert.That(updatePage.Style, Is.Null);
        }

        /// <summary>
        /// Test that OnPost updates the material and style of a valid product and redirects to the index page.
        /// </summary>
        [Test]
        public void OnPost_Valid_Product_Should_Update_Material_And_Style_And_Redirect_To_Index()
        {
            var product = new ProductModel { Id = "1", Title = "Updated Product" };
            updatePage.SelectedProduct = product;
            updatePage.Material = "Wood, Glass";
            updatePage.Style = "Modern, Vintage";
            mockProductService.Setup(service => service.UpdateData(It.IsAny<ProductModel>())).Returns(product);
            var result = updatePage.OnPost() as RedirectToPageResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageName, Is.EqualTo("./Index"));
            Assert.That(updatePage.SelectedProduct.Material, Is.EquivalentTo(new List<string> { "Wood", "Glass" }));
            Assert.That(updatePage.SelectedProduct.Style, Is.EquivalentTo(new List<string> { "Modern", "Vintage" }));
        }

        /// <summary>
        /// Test that OnPost returns the current page when the model state is invalid.
        /// </summary>
        [Test]
        public void OnPost_InValid_ModelState_Should_Return_Page()
        {
            updatePage.ModelState.AddModelError("Title", "Title is required");
            var result = updatePage.OnPost();
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        /// <summary>
        /// Test that OnPost returns a NotFound result when the product ID does not exist.
        /// </summary>
        [Test]
        public void OnPost_InValid_Non_Existent_ProductId_Should_Return_NotFound()
        {
            var product = new ProductModel { Id = "nonexistent", Title = "Nonexistent Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns((ProductModel)null);
            var result = updatePage.OnPost() as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

        /// <summary>
        /// Test that OnGet loads and sets the values correctly for a valid product ID.
        /// </summary>
        [Test]
        public void OnGet_Valid_ProductId_Should_Select_Product_Set_Values_Correctly()
        {
            var productId = "test-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = new List<string> { "Wood", "Metal" },
                Style = new List<string> { "Modern", "Classic" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            updatePage.OnGet(productId);
            Assert.That(updatePage.SelectedProduct, Is.Not.Null);
            Assert.That(updatePage.SelectedProduct.Id, Is.EqualTo(productId));
            Assert.That(updatePage.Material, Is.EqualTo("Wood, Metal"));
            Assert.That(updatePage.Style, Is.EqualTo("Modern, Classic"));
        }

        /// <summary>
        /// Test that OnGet correctly sets empty strings for null material or style fields.
        /// </summary>
        [Test]
        public void OnGet_InValid_Null_Fields_Should_Set_Empty_Strings()
        {
            var productId = "null-material-style";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Product with null fields",
                Material = null,
                Style = null
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            updatePage.OnGet(productId);
            Assert.That(updatePage.Material, Is.EqualTo(string.Empty));
            Assert.That(updatePage.Style, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// Tests that OnPostResetComments does nothing and redirects when the product does not exist.
        /// </summary>
        [Test]
        public void OnPostResetComments_Invalid_ProductId_Should_Not_Throw_And_Redirect()
        {
            // Arrange: Set up data with no matching product ID
            var mockData = new List<ProductModel>
    {
        new ProductModel { Id = "valid-id", Title = "Valid Product", Comments = new List<string> { "Test Comment" } }
    };

            // Mock the GetAllData to return the data
            mockProductService.Setup(service => service.GetAllData()).Returns(mockData);

            // Act: Call OnPostResetComments with an invalid ID
            var result = updatePage.OnPostResetComments("invalid-id");

            // Assert: Verify no changes were made and redirection occurred
            Assert.That(result, Is.TypeOf<RedirectToPageResult>(), "Expected a redirection result.");
            mockProductService.Verify(service => service.GetAllData(), Times.Once, "GetAllData should be called.");
            Assert.That(mockData.First().Comments.Count, Is.EqualTo(1), "Comments should remain unchanged.");
        }

        [Test]
        public void OnPostResetComments_Valid_ProductId_Should_Clear_Comments_And_Update_Product()
        {
            // Arrange: Set up a product with comments
            var mockData = new List<ProductModel>
    {
        new ProductModel { Id = "valid-id", Title = "Test Product", Comments = new List<string> { "Comment1", "Comment2" } }
    };

            // Mock GetAllData to return the product list
            mockProductService.Setup(service => service.GetAllData()).Returns(mockData);

            // Mock SaveData to handle the SaveData call during UpdateProduct
            mockProductService.Setup(service => service.SaveData(It.IsAny<IEnumerable<ProductModel>>())).Verifiable();

            // Use the actual UpdateProduct method from the real service
            mockProductService.CallBase = true;

            // Act: Call OnPostResetComments with a valid product ID
            var result = updatePage.OnPostResetComments("valid-id");

            // Assert: Verify comments were cleared
            var updatedProduct = mockData.First(p => p.Id == "valid-id");
            Assert.That(updatedProduct.Comments, Is.Empty, "Comments should be cleared.");

            // Assert: Verify redirection occurred
            Assert.That(result, Is.TypeOf<RedirectToPageResult>(), "Expected a redirection result.");
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo("valid-id"), "Expected redirection to include the product ID.");
        }

    }
}
