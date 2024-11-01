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
    public class DeleteTests
    {
        private Mock<JsonFileProductService> mockProductService;
        private DeleteModel DeletePage;

        [SetUp]
        public void SetUp()
        {
            // Mocking the ProductService to avoid dependency on external data
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, new Mock<IWebHostEnvironment>().Object);

            // Initialize DeleteModel with the mocked JsonFileProductService
            DeletePage = new DeleteModel(mockProductService.Object);
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
            DeletePage.OnGet("1");

            // Assert
            Assert.That(DeletePage.SelectedProduct, Is.Not.Null);
            Assert.That(DeletePage.SelectedProduct.Title, Is.EqualTo("Product1"));
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
            DeletePage.OnGet("invalid");

            // Assert
            Assert.That(DeletePage.SelectedProduct, Is.Null);
        }

        [Test]
        public void OnPost_Valid_Product_Should_Delete_And_Redirect_To_Index()
        {
            // Arrange
            var product = new ProductModel { Id = "1", Title = "Product to Delete" };
            DeletePage.SelectedProduct = product;
            // mockProductService.Setup(service => service.DeleteData(product.Id)).Returns(true);

            // Act
            var result = DeletePage.OnPost() as RedirectToPageResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageName, Is.EqualTo("./Index"));
        }

        [Test]
        public void OnPost_InValid_Model_State_Should_Return_Page()
        {
            // Arrange
            DeletePage.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = DeletePage.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        [Test]
        public void OnPost_InValid_Non_Existent_Product_Should_Return_NotFound()
        {
            // Arrange
            var product = new ProductModel { Id = "nonexistent", Title = "Nonexistent Product" };
            DeletePage.SelectedProduct = product;
            mockProductService.Setup(service => service.DeleteData(product.Id)).Returns((ProductModel)null); // Return null to simulate non-existent product

            // Act
            var result = DeletePage.OnPost() as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
        }


    }
}
