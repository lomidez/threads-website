using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UnitTests.Pages
{
    [TestFixture]
    public class UpdateTests
    {
        private Mock<JsonFileProductService> mockProductService;
        private UpdateModel updatePage;

        [SetUp]
        public void SetUp()
        {
            // Mocking the ProductService to avoid dependency on external data
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, new Mock<IWebHostEnvironment>().Object);

            // Initialize UpdateModel with the mocked JsonFileProductService
            updatePage = new UpdateModel(mockProductService.Object);
        }

        [Test]
        public void OnGet_Valid_Product_Id_Should_Return_Product()
        {
            // Arrange
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act
            updatePage.OnGet("1");

            // Assert
            Assert.That(updatePage.SelectedProduct, Is.Not.Null);
            Assert.That(updatePage.SelectedProduct.Title, Is.EqualTo("Product1"));
        }

        [Test]
        public void OnGet_InValid_Product_Id_Should_Return_Null()
        {
            // Arrange
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act
            updatePage.OnGet("invalid");

            // Assert
            Assert.That(updatePage.SelectedProduct, Is.Null);
        }

        [Test]
        public void OnPost_Valid_Product_Should_Update_And_Redirect_To_Index()

        {
            // Arrange
            var product = new ProductModel { Id = "1", Title = "Updated Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns(product);

            // Act
            var result = updatePage.OnPost() as RedirectToPageResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageName, Is.EqualTo("./Index"));
        }

        [Test]
        public void OnPost_InValid_Model_State_Should_Return_Page()

        {
            // Arrange
            updatePage.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = updatePage.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        [Test]
        public void OnPost_InValid_Non_Existent_Product_Should_Return_NotFound()
        {
            // Arrange
            var product = new ProductModel { Id = "nonexistent", Title = "Nonexistent Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns((ProductModel)null);

            // Act
            var result = updatePage.OnPost() as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }
    }
}
