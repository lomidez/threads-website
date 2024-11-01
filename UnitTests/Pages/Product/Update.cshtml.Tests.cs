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
    /// <summary>
    /// Unit tests for the Update page model, covering functionality for retrieving and updating product data.
    /// </summary>
    [TestFixture]
    public class UpdateTests
    {
        private Mock<JsonFileProductService> mockProductService; // Mocked product service to avoid external dependency
        private UpdateModel updatePage; // Instance of the UpdateModel page for testing

        /// <summary>
        /// Sets up mock dependencies and initializes the UpdateModel instance before each test.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Arrange: Mocking the ProductService to avoid dependency on external data
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, new Mock<IWebHostEnvironment>().Object);

            // Initialize UpdateModel with the mocked JsonFileProductService
            updatePage = new UpdateModel(mockProductService.Object);
        }

        /// <summary>
        /// Tests that a valid product ID retrieves the corresponding product.
        /// </summary>
        [Test]
        public void OnGet_Valid_Product_Id_Should_Return_Product()
        {
            // Arrange: Mocking product list containing a specific product ID
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Calling OnGet with a valid product ID
            updatePage.OnGet("1");

            // Assert: Checking that the correct product is returned
            Assert.That(updatePage.SelectedProduct, Is.Not.Null);
            Assert.That(updatePage.SelectedProduct.Title, Is.EqualTo("Product1"));
        }

        /// <summary>
        /// Tests that an invalid product ID returns null as the selected product.
        /// </summary>
        [Test]
        public void OnGet_InValid_Product_Id_Should_Return_Null()
        {
            // Arrange: Mocking product list with a specific valid product ID
            var productList = new List<ProductModel>
            {
                new ProductModel { Id = "1", Title = "Product1" }
            };
            mockProductService.Setup(service => service.GetAllData()).Returns(productList);

            // Act: Calling OnGet with an invalid product ID
            updatePage.OnGet("invalid");

            // Assert: Verifying that no product is found
            Assert.That(updatePage.SelectedProduct, Is.Null);
        }

        /// <summary>
        /// Tests that a valid product update redirects to the Index page upon submission.
        /// </summary>
        [Test]
        public void OnPost_Valid_Product_Should_Update_And_Redirect_To_Index()
        {
            // Arrange: Setting up product data to be updated and expected redirect result
            var product = new ProductModel { Id = "1", Title = "Updated Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns(product);

            // Act: Calling OnPost to submit the update
            var result = updatePage.OnPost() as RedirectToPageResult;

            // Assert: Verifying that the result redirects to Index
            Assert.That(result, Is.Not.Null);
            Assert.That(result.PageName, Is.EqualTo("./Index"));
        }

        /// <summary>
        /// Tests that submitting an invalid model state returns the current page for correction.
        /// </summary>
        [Test]
        public void OnPost_InValid_Model_State_Should_Return_Page()
        {
            // Arrange: Adding a model error to simulate an invalid state
            updatePage.ModelState.AddModelError("Title", "Title is required");

            // Act: Calling OnPost to submit with invalid model state
            var result = updatePage.OnPost();

            // Assert: Verifying that a PageResult is returned
            Assert.That(result, Is.TypeOf<PageResult>());
        }

        /// <summary>
        /// Tests that a non-existent product ID returns a NotFound result.
        /// </summary>
        [Test]
        public void OnPost_InValid_Non_Existent_Product_Should_Return_NotFound()
        {
            // Arrange: Setting up a non-existent product to simulate a failed update
            var product = new ProductModel { Id = "nonexistent", Title = "Nonexistent Product" };
            updatePage.SelectedProduct = product;
            mockProductService.Setup(service => service.UpdateData(product)).Returns((ProductModel)null);

            // Act: Calling OnPost with a non-existent product ID
            var result = updatePage.OnPost() as NotFoundResult;

            // Assert: Verifying that a NotFound result is returned
            Assert.That(result, Is.Not.Null);
        }
    }
}
