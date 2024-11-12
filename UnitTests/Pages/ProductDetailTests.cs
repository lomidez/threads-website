using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Pages
{
    [TestFixture]
    public class ProductDetailsTests
    {
        private Mock<JsonFileProductService> mockProductService;
        private ProductDetailsModel productDetailsPage;

        [SetUp]
        public void SetUp()
        {
            // Mock the product service
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, null);
            productDetailsPage = new ProductDetailsModel(mockProductService.Object);
        }
        [Test]
        public void OnPostAddLike_Should_Increment_Like_And_Redirect()
        {
            // Arrange
            var productId = "test-id";

            // Mock AddLike to return true for the given productId
            mockProductService.Setup(service => service.AddLike(productId)).Returns(true);

            // Act: Call OnPostAddLike with the test productId
            var result = productDetailsPage.OnPostAddLike(productId);

            // Assert: Verify that AddLike was called
            mockProductService.Verify(service => service.AddLike(productId), Times.Once);

            // Assert: Check if the result is a redirect to ProductDetails page with the correct ID
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult.PageName, Is.EqualTo("/ProductDetails"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(productId));
        }


    }
}
