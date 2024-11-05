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





namespace UnitTests.Pages.Products
{
    public class CreateTests
    {
        private CreateModel pageModel;
        private Mock<JsonFileProductService> mockProductService;
        private List<ProductModel> mockProducts;

        [SetUp]
        public void TestInitialize()
        {
            // Set up a mock product list
            mockProducts = new List<ProductModel>
            {
                new ProductModel { Id = "existing-id", Title = "Existing Product" }
            };

            // Set up mock environment and service
            mockProductService = new Mock<JsonFileProductService>(new Mock<IWebHostEnvironment>().Object);
            mockProductService.Setup(service => service.GetAllData()).Returns(mockProducts);
            pageModel = new CreateModel(mockProductService.Object);
           pageModel.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

        }

       



        [Test]
        public void OnPost_valid_DuplicateId_Should_Return_Page_With_Error()
        {
            // Arrange
            pageModel.NewProduct = new ProductModel { Id = "existing-id", Title = "New Product" };

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState["NewProduct.Id"].Errors.Count, Is.GreaterThan(0));
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Never);
        }

        



        [Test]
        public void OnPost_CreateDataFails_Should_Return_Page_With_Error()
        {
            // Arrange - Simulate a save failure
            pageModel.NewProduct = new ProductModel { Id = "new-id", Title = "New Product" };
            mockProductService.Setup(service => service.CreateData(It.IsAny<ProductModel>())).Returns(false);

            // Act
            var result = pageModel.OnPost();

            // Assert
            Assert.That(result, Is.TypeOf<PageResult>());
            Assert.That(pageModel.ModelState[string.Empty].Errors.Count, Is.GreaterThan(0));
            mockProductService.Verify(service => service.CreateData(It.IsAny<ProductModel>()), Times.Once);
        }

















    }
}
