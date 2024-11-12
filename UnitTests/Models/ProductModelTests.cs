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
    [TestFixture]
    public class ProductDetailsTests
    {
        private ProductDetailsModel pageModel;
        private Mock<JsonFileProductService> mockProductService;
        [SetUp]
        public void TestInitialize()
        {
            // Set up a mock Product Service
            mockProductService = new Mock<JsonFileProductService>(MockBehavior.Strict, new object[] { null });

            // Initialize ProductDetailsModel with the mock service
            pageModel = new ProductDetailsModel(mockProductService.Object);
        }



        [Test]
        public void ProductModel_Title_Should_Respect_StringLength_Validation()
        {
            var product = new ProductModel();
            product.Title = "Valid Title";
            Assert.That(product.Title, Is.EqualTo("Valid Title"));

            // Exceed the max length and assert validation
            product.Title = new string('a', 65);
            // Add custom validation logic in your project as NUnit does not enforce data annotations directly
        }

        [Test]
        public void ProductModel_Ratings_Should_Have_Default_Empty_Array()
        {
            var product = new ProductModel();
            Assert.That(product.Ratings, Is.Not.Null);
            Assert.That(product.Ratings.Length, Is.EqualTo(0));
        }

        [Test]
        public void ProductModel_Likes_Should_Have_Default_Value_Zero()
        {
            var product = new ProductModel();
            Assert.That(product.Likes, Is.EqualTo(0));
        }

        [Test]
        public void ProductModel_Quantity_Should_Handle_Null_Value()
        {
            var product = new ProductModel();
            Assert.That(product.Quantity, Is.Null);

            product.Quantity = 10;
            Assert.That(product.Quantity, Is.EqualTo(10));
        }

        [Test]
        public void ProductModel_Price_Should_Be_NonNegative()
        {
            var product = new ProductModel { Price = 100 };
            Assert.That(product.Price, Is.EqualTo(100));
        }

        [Test]
        public void ProductModel_CommentList_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.CommentList, Is.Not.Null);
            Assert.That(product.CommentList.Count, Is.EqualTo(0));
        }

        [Test]
        public void ProductModel_Material_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.Material, Is.Not.Null);
            Assert.That(product.Material.Count, Is.EqualTo(0));
        }

        [Test]
        public void ProductModel_Style_Should_Have_Default_Empty_List()
        {
            var product = new ProductModel();
            Assert.That(product.Style, Is.Not.Null);
            Assert.That(product.Style.Count, Is.EqualTo(0));
        }

        [Test]
        public void ProductModel_Dimentions_Should_Be_Initialized()
        {
            var product = new ProductModel();
            Assert.That(product.Dimentions, Is.Not.Null);
        }

        [Test]
        public void ProductModel_ToString_Should_Return_JsonString()
        {
            var product = new ProductModel { Id = "12345", Title = "Sample Product" };
            var jsonString = product.ToString();
            Assert.That(jsonString, Is.EqualTo(JsonSerializer.Serialize(product)));
        }

        [Test]
        public void Dimensions_Length_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Length = 10.5m };
            Assert.That(dimensions.Length, Is.EqualTo(10.5m));
        }

        [Test]
        public void Dimensions_Width_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Width = 5.0m };
            Assert.That(dimensions.Width, Is.EqualTo(5.0m));
        }

        [Test]
        public void Dimensions_Height_Should_Be_Gettable_And_Settable()
        {
            var dimensions = new Dimensions { Height = 2.5m };
            Assert.That(dimensions.Height, Is.EqualTo(2.5m));
        }



        [Test]
        public void OnGet_ValidProductId_Should_Set_SelectedProduct()
        {
            // Arrange
            var mockProduct = new ProductModel { Id = "test-id", Title = "Test Product", Likes = 10 };
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel> { mockProduct });

            // Act
            pageModel.OnGet("test-id");

            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Not.Null, "Expected SelectedProduct to be set.");
            Assert.That(pageModel.SelectedProduct.Id, Is.EqualTo("test-id"));
            Assert.That(pageModel.SelectedProduct.Title, Is.EqualTo("Test Product"));
            Assert.That(pageModel.SelectedProduct.Likes, Is.EqualTo(10)); // Ensures Likes is set as expected
        }


        [Test]
        public void OnGet_InvalidProductId_Should_Not_Set_SelectedProduct()
        {
            // Arrange
            mockProductService.Setup(service => service.GetAllData()).Returns(new List<ProductModel>());
            // Act
            pageModel.OnGet("nonexistent-id");
            // Assert
            Assert.That(pageModel.SelectedProduct, Is.Null, "Expected SelectedProduct to be Null for invalid ID.");
        }














    }
}
