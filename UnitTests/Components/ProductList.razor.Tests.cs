using Bunit;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System.Linq;
using System.Text.Json;
using Moq;
using Microsoft.AspNetCore.Hosting;
using ContosoCrafts.WebSite.Components;
using System.Collections.Generic;

namespace ContosoCrafts.WebSite.Tests
{
    public class ProductListTests : Bunit.TestContext
    {
        private Mock<JsonFileProductService> _mockProductService;
        private ProductModel[] _testProducts;

        [SetUp]
        public void Setup()
        {
            // Create mock test data
            _testProducts = new[]
            {
                new ProductModel
                {
                    Id = "woven-tote",
                    Title = "Woven Tote",
                    Description = "Details about woven tote",
                    Image = "/data/images/tote.jpg",
                    Likes = 2,
                    Category = "bag",
                    Size = "large",
                    Color = "brown",
                    Material = new List<string> { "leather", "straw" },
                    Style = new List<string> { "casual", "summer" },
                    Comments = new List<string>()
                },
                new ProductModel
                {
                    Id = "white-bag",
                    Title = "Minimalist White City Backpack",
                    Description = "Details about white backpack",
                    Image = "/data/images/backpack.jpg",
                    Likes = 1,
                    Category = "bag",
                    Size = "large",
                    Color = "white",
                    Material = new List<string> { "nylon" },
                    Style = new List<string> { "sporty", "minimalist" },
                    Comments = new List<string>()
                }
            };

            // Create mock service
            _mockProductService = new Mock<JsonFileProductService>(Mock.Of<IWebHostEnvironment>());

            // Setup mock methods
            _mockProductService.Setup(s => s.GetAllData())
                .Returns(_testProducts);

            // Register the mock service
            Services.AddSingleton<JsonFileProductService>(_mockProductService.Object);
        }

        [Test]
        public void ProductList_Default_Should_Return_Content()
        {
            // Act
            var cut = RenderComponent<ProductList>();
            var result = cut.Markup;

            // Assert
            Assert.That(result, Does.Contain("Woven Tote"));
        }

        [Test]
        public void FilterProduct_White_Should_Not_Return_Tote()
        {
            // Arrange
            var id = "white";

            // Act
            var cut = RenderComponent<ProductList>();

            // Find all buttons and click the one matching our ID
            var buttonList = cut.FindAll(".category-value");
            var button = buttonList.First(m => m.TextContent == id);
            button.Click();

            var results = cut.Markup;

            // Woven-tote shows up in trending products too, we want to see if there is now one occurance instead of two
            var occurances = results.Split("woven-tote").Length - 1;
            
            // Assert
            Assert.That(occurances, Is.LessThan(2));
        }

        [Test]
        public void ProductList_Like_Button_Should_Increment_Likes()
        {
            // Act
            var cut = RenderComponent<ProductList>();

            // Find the first like button and click it
            var firstProduct = cut.FindAll("button.like-button").First();
            var initialLikes = int.Parse(firstProduct.QuerySelector(".like-count").TextContent);

            firstProduct.Click();

            var finalLikes = int.Parse(firstProduct.QuerySelector(".like-count").TextContent);

            // Assert
            Assert.That(finalLikes, Is.EqualTo(initialLikes + 1));
        }
    }
}
