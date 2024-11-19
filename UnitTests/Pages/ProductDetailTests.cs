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
        [Test]
        public void OnePost_AddLike_Should_Call_AddLike_With_Valid_Id_And_Redirect ()
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



        






        /// <summary>
        /// Test that the NewComment property works as expected.
        /// </summary>
        [Test]
        public void NewComment_Should_Store_Comment()
        {
            // Arrange: Set a test comment
            var testComment = "This is a new comment.";

            // Act: Assign the comment to the NewComment property
            productDetailsPage.NewComment = testComment;

            // Assert: Verify that the NewComment property stores the comment correctly
            Assert.That(productDetailsPage.NewComment, Is.EqualTo(testComment));
        }

        /// <summary>
        /// Test that OnPostAddComment adds a comment and clears the input.
        /// </summary>
        [Test]
        public void OnPostAddComment_Valid_Comment_Should_Add_Comment_And_Clear_Input()
        {
            // Arrange: Set a valid comment and product ID
            var productId = "test-id";
            var comment = "This is a valid comment.";
            productDetailsPage.NewComment = comment;

            // Mock the AddComment method
            mockProductService.Setup(service => service.AddComment(productId, comment));

            // Act: Call OnPostAddComment with the test productId
            var result = productDetailsPage.OnPostAddComment(productId);


            // Assert: Check if the NewComment property was cleared
            Assert.That(productDetailsPage.NewComment, Is.Empty);

            // Assert: Check if the result is a redirect to the ProductDetails page with the correct ID
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult.PageName, Is.EqualTo("/ProductDetails"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(productId));
        }

        /// <summary>
        /// Test that OnPostAddComment does not add an empty or whitespace comment.
        /// </summary>
        [Test]
        public void OnPostAddComment_Invalid_Comment_Should_Not_Add_Comment()
        {
            // Arrange: Set an invalid (empty) comment and product ID
            var productId = "test-id";
            productDetailsPage.NewComment = "   "; // Whitespace comment

            // Act: Call OnPostAddComment with the test productId
            var result = productDetailsPage.OnPostAddComment(productId);

            // Assert: Verify that AddComment was not called
            mockProductService.Verify(service => service.AddComment(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            // Assert: Check if the result is a redirect to the ProductDetails page with the correct ID
            Assert.That(result, Is.TypeOf<RedirectToPageResult>());
            var redirectResult = result as RedirectToPageResult;
            Assert.That(redirectResult.PageName, Is.EqualTo("/ProductDetails"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(productId));
        }
    }
}

