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
    /// <summary>
    /// Unit tests for the ProductDetailsModel class in the ProductDetails page of the application.
    /// </summary>
    [TestFixture]
    public class ProductDetailsTests
    {
        private Mock<JsonFileProductService> mockProductService; ///<summary>Mock of JsonFileProductService used in testing.</summary>
        private ProductDetailsModel productDetailsPage; ///<summary>Instance of ProductDetailsModel for testing.</summary>

        /// <summary>
        /// Setup method to initialize mocks and the ProductDetailsModel instance before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Mock the product service
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, null);
            productDetailsPage = new ProductDetailsModel(mockProductService.Object);
        }

        /// <summary>
        /// Test that the OnPostAddLike method correctly increments likes and redirects to the ProductDetails page.
        /// </summary>
        /// <remarks>
        /// Verifies that calling OnPostAddLike with a productId increments the like count and performs a redirect to the ProductDetails page for the given product.
        /// </remarks>
        [Test]
        public void OnPostAddLike_Should_Increment_Like_And_Redirect()
        {
            // Arrange: Initialize the test data
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
