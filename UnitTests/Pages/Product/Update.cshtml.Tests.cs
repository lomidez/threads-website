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
namespace UnitTests.Pages
{
    [TestFixture]
    public class UpdateTests
    {
        private Mock<IWebHostEnvironment> mockEnvironment;
        private Mock<JsonFileProductService> mockProductService;
        private UpdateModel updatePage;
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

            // Assert: Verify ResetLikes was called with the correct product ID
            mockProductService.Verify(service => service.ResetLikes(validProductId), Times.Once);

            // Check that the method redirects correctly
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult, Is.Not.Null, "Expected a redirection result.");
            Assert.That(redirectResult.PageName, Is.EqualTo("/Product/Update"), "Expected redirection to the Update page.");
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(validProductId), "Expected the ID route value to match the product ID.");
        }









        [Test]
        public void OnPostResetLikes_InvalidProductId_Should_Throw_InvalidOperationException()
        {
            var invalidProductId = "invalid-id";
            mockProductService
                .Setup(service => service.ResetLikes(invalidProductId))
                .Throws(new InvalidOperationException("product not found"));
            var exception = Assert.Throws<InvalidOperationException>(() => updatePage.OnPostResetLikes(invalidProductId));
            Assert.That(exception.Message, Is.EqualTo("product not found"));
        }

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

        [Test]
        public void OnGet_Invalid_ProductId_Should_Not_Load_Product()
        {
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());
            updatePage.OnGet("invalid");
            Assert.That(updatePage.SelectedProduct, Is.Null);
            Assert.That(updatePage.Material, Is.Null);
            Assert.That(updatePage.Style, Is.Null);
        }

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

        [Test]
        public void OnPost_InValid_ModelState_Should_Return_Page()
        {
            updatePage.ModelState.AddModelError("Title", "Title is required");
            var result = updatePage.OnPost();
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        [Test]
        public void OnPost_InValid_Non_Existent_ProductId_Should_Return_NotFound()
        {
            var product = new ProductModel { Id = "nonexistent", Title = "Nonexistent Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns((ProductModel)null);
            var result = updatePage.OnPost() as NotFoundResult;
            Assert.That(result, Is.Not.Null);
        }

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

        [Test]
        public void OnGet_Valid_ProductId_Should_Set_Values_Correctly()
        {
            var productId = "valid-id";
            var product = new ProductModel
            {
                Id = productId,
                Title = "Test Product",
                Material = new List<string> { "Wood", "Metal" },
                Style = new List<string> { "Modern", "Classic" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { product });
            updatePage.OnGet(productId);
            Assert.That(updatePage.Material, Is.EqualTo("Wood, Metal"), "Expected Material to be set as 'Wood, Metal'");
            Assert.That(updatePage.Style, Is.EqualTo("Modern, Classic"), "Expected Style to be set as 'Modern, Classic'");
        }

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
    }
}
