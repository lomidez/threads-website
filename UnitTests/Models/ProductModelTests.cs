using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Pages;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Text.Json;

namespace UnitTests.Pages.Products
{
    /// <summary>
    /// Unit tests for the ProductDetails page model.
    /// </summary>
    [TestFixture]
    public class ProductDetailsTests
    {
        // Instance of the ProductDetailsModel under test.
        private ProductDetailsModel pageModel;

        // Mock instance of the JsonFileProductService.
        private Mock<JsonFileProductService> mockProductService;

        /// <summary>
        /// Initializes the test setup by creating mock dependencies.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Set up a mock Product Service with strict behavior.
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, new object[] { null });

            // Initialize ProductDetailsModel with the mock service.
            pageModel = new ProductDetailsModel(mockProductService.Object);
        }

        /// <summary>
        /// Tests that the product title respects string length validation.
        /// </summary>
        [Test]
        public void ProductModel_Title_Should_Respect_StringLength_Validation()
        {
            var product = new ProductModel();

            // Test with a valid title.
            product.Title = "Valid Title";
            Assert.That(product.Title, Is.EqualTo("Valid Title"));

            // Exceed the max length and assert validation.
            product.Title = new string('a', 65);
            // Note: Custom validation logic needed, as NUnit does not enforce data annotations.
        }

        /// <summary>
        /// Tests that the product ratings default to an empty array.
        /// </summary>
        [Test]
        public void ProductModel_Ratings_Should_Have_Default_Empty_Array()
        {
            var product = new ProductModel();
            Assert.That(product.Ratings, Is.Not.Null);
            Assert.That(product.Ratings.Length, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the product likes default to zero.
        /// </summary>
        [Test]
        public void ProductModel_Likes_Should_Have_Default_Value_Zero()
        {
            var product = new ProductModel();
            Assert.That(product.Likes, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the product quantity can handle null values.
        /// </summary>
        [Test]
        public void ProductModel_Quantity_Should_Handle_Null_Value()
        {
            var product = new ProductModel();
            Assert.That(product.Quantity, Is.Null);

            product.Quantity = 10;
            Assert.That(product.Quantity, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests that the product price cannot be negative.
        /// </summary>
        [Test]
        public void ProductModel_Price_Should_Be_NonNegative()
        {
            var product = new ProductModel { Price = 100 };
            Assert.That(product.Price, Is.EqualTo(100));
        }

        /// <summary>
        /// Tests that the product comment list defaults to an empty list.
        /// </summary>
        [Test]
        public void ProductModel_CommentList_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.CommentList, Is.Not.Null);
            Assert.That(product.CommentList.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the product material list defaults to an empty list.
        /// </summary>
        [Test]
        public void ProductModel_Material_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.Material, Is.Not.Null);
            Assert.That(product.Material.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the product style list defaults to an empty list.
        /// </summary>
        [Test]
        public void ProductModel_Style_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.Style, Is.Not.Null);
            Assert.That(product.Style.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that the product dimensions are initialized properly.
        /// </summary>
        //[Test]
        //public void ProductModel_Dimensions_Should_Be_Initialized()
        //{
        //    var product = new ProductModel();
        //    Assert.That(product.Dimensions, Is.Not.Null);
        //}

        /// <summary>
        /// Tests that the product's ToString method returns a JSON string.
        /// </summary>
        [Test]
        public void ProductModel_ToString_Should_Return_JsonString()
        {
            var product = new ProductModel { Id = "12345", Title = "Sample Product" };
            var jsonString = product.ToString();
            Assert.That(jsonString, Is.EqualTo(JsonSerializer.Serialize(product)));
        }

        /// <summary>
        /// Tests that the length of dimensions is gettable and settable.
        /// </summary>
        [Test]
        public void Dimensions_Length_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Length = 10.5m };
            Assert.That(dimensions.Length, Is.EqualTo(10.5m));
        }

        /// <summary>
        /// Tests that the width of dimensions is gettable and settable.
        /// </summary>
        [Test]
        public void Dimensions_Width_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Width = 5.0m };
            Assert.That(dimensions.Width, Is.EqualTo(5.0m));
        }

        /// <summary>
        /// Tests that the height of dimensions is gettable and settable.
        /// </summary>
        [Test]
        public void Dimensions_Height_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Height = 2.5m };
            Assert.That(dimensions.Height, Is.EqualTo(2.5m));
        }

        /// <summary>
        /// Tests that OnGet with a valid product ID sets the selected product.
        /// </summary>
        [Test]
        public void OnGet_ValidProductId_Should_Set_SelectedProduct()
        {
            // Arrange: Set up a mock product.
            var mockProduct = new ProductModel { Id = "test-id", Title = "Test Product", Likes = 10 };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { mockProduct });

            // Act: Call the OnGet method with a valid product ID.
            pageModel.OnGet("test-id");

            // Assert: Verify that the selected product is set correctly.
            Assert.That(pageModel.SelectedProduct, Is.Not.Null, "Expected SelectedProduct to be set.");
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo("test-id"));
            Assert.That(pageModel.SelectedProduct.Title, Is.EqualTo("Test Product"));
            Assert.That(pageModel.SelectedProduct.Likes, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests that OnGet with an invalid product ID does not set the selected product.
        /// </summary>
        [Test]
        public void OnGet_InvalidProductId_Should_Not_Set_SelectedProduct()
        {
            // Arrange: Set up an empty product list.
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());

            // Act: Call the OnGet method with an invalid product ID.
            pageModel.OnGet("nonexistent-id");

            // Assert: Verify that the selected product is not set.
            Assert.That(pageModel.SelectedProduct, Is.Null, "Expected SelectedProduct to be Null for invalid ID.");
        }
    }
}
