using Bunit;
using ContosoCrafts.WebSite.Components;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Tests
{
    public class ProductListTests : Bunit.TestContext
    {
        private Mock<JsonFileProductService> _mockProductService;
        private ProductModel[] _testProducts;

        public ProductListTests()
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
                    Category = ProductCategory.bag,
                    Size = ProductSize.Large,
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
                    Category = ProductCategory.bag,
                    Size = ProductSize.Large,
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
            // Arrange

            // Act
            var cut = RenderComponent<ProductList>();
            var result = cut.Markup;

            // Assert
            Assert.That(result, Does.Contain("Minimalist White City Backpack"));
        }


        [Test]
        public void FilterProduct_White_Should_Not_Return_Tote()
        {
            // Arrange
            var id = "white";

            // Act
            var cut = RenderComponent<ProductList>();

            // Find all buttons and click the one matching our ID
            var buttonList = cut.FindAll(".category-text");
            var button = buttonList.First(m => m.TextContent == id);
            button.Click();

            var results = cut.Markup;

            // Woven-tote shows up in trending products too, we want to see if there is now one occurance instead of two
            var occurances = results.Split("woven-tote").Length - 1;

            // Assert
            Assert.That(occurances, Is.LessThan(2));
        }

        [Test]
        public void FilterProduct_Large_Should_Return_Tote()
        {
            // Arrange
            var id = "Large";

            // Act
            var cut = RenderComponent<ProductList>();

            // Find all buttons and click the one matching our ID
            var buttonList = cut.FindAll(".category-text");
            var button = buttonList.First(m => m.TextContent == id);
            button.Click();

            var results = cut.Markup;

            // Woven-tote shows up in trending products too, we want to see if there is now one occurance instead of two
            var occurances = results.Split("woven-tote").Length - 1;

            // Assert
            Assert.That(occurances, Is.AtLeast(2));
        }

        [Test]
        public void FilterProduct_Bag_Should_Return_Tote()
        {
            // Arrange
            var id = "bag";

            // Act
            var cut = RenderComponent<ProductList>();

            // Find all buttons and click the one matching our ID
            var buttonList = cut.FindAll(".category-text");
            var button = buttonList.First(m => m.TextContent == id);
            button.Click();

            var results = cut.Markup;

            // Woven-tote shows up in trending products too, we want to see if there is now one occurance instead of two
            var occurances = results.Split("woven-tote").Length - 1;

            // Assert
            Assert.That(occurances, Is.AtLeast(2));
        }

        [Test]
        public void ProductList_Invalid_Search_Should_Not_Show_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");
            
            //Act
            searchInput.Input("Gibberish");

            searchInput.KeyDown("Enter");

            var result = cut.Markup;

            //Assert
            Assert.That(result, Does.Contain("No products found"));
        }

        [Test]
        public void ProductList_Empty_Search_Should_Not_Show_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");
            searchInput.Input("");

            var previousResults = cut.Markup;

            //Act
            searchInput.KeyDown("Enter");

            var result = cut.Markup;

            //Assert
            Assert.That(result.Length, Is.EqualTo(previousResults.Length));
        }

        [Test]
        public void ProductList_Valid_Search_Should_Show_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");

            //Act
            searchInput.Input("Tote");

            searchInput.KeyDown("Enter");

            var result = cut.Markup;

            //Assert
            Assert.That(result, Does.Contain("woven-tote"));
        }

        [Test]
        public void ProductList_Valid_Substring_Search_Should_Show_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");

            //Act
            searchInput.Input("bags");

            searchInput.KeyDown("Enter");

            var result = cut.Markup;

            //Assert
            Assert.That(result, Does.Contain("woven-tote"));
        }

        [Test]
        public void ProductList_Valid_Split_Search_Should_Show_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");

            //Act
            searchInput.Input("brown, tote");

            searchInput.KeyDown("Enter");

            var result = cut.Markup;

            //Assert
            Assert.That(result, Does.Contain("woven-tote"));
        }

        [Test]
        public void TrendingProducts_Next_Button_Should_Increment_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var oldResults = cut.Markup;

            //Act            
            var buttonList = cut.FindAll(".carousel-button");
            var button = buttonList[0];
            button.Click();

            var newResults = cut.Markup;

            //Assert
            Assert.That(newResults, Does.Not.EqualTo(oldResults));
        }

        [Test]
        public void TrendingProducts_Previous_Button_Should_Increment_Products()
        {
            //Arrange
            var cut = RenderComponent<ProductList>();

            var oldResults = cut.Markup;
            
            //Act
            var buttonList = cut.FindAll(".carousel-button");
            var button = buttonList[1];
            button.Click();

            var newResults = cut.Markup;

            //Assert
            Assert.That(newResults, Does.Not.EqualTo(oldResults));
        }

        [Test]
        public void Sort_Ascending_Should_Order_Products_By_Likes()
        {
            // Arrange
            var cut = RenderComponent<ProductList>();

            // Act - Find and click the ascending sort button (↑)
            var sortButtons = cut.FindAll(".btn-arrow");
            var ascButton = sortButtons.First(b => b.TextContent == "↑");
            ascButton.Click();

            // Get all like counts
            var likeCounts = cut.FindAll(".like-count")
                .Select(e => int.Parse(e.TextContent))
                .ToList();

            // Assert - Check if the likes are in ascending order
            Assert.That(likeCounts, Is.Ordered);
        }

        [Test]
        public void Sort_Ascending_Twice_Should_Not_Order_Products_By_Likes()
        {
            // Arrange
            var cut = RenderComponent<ProductList>();

            // Act - Find and click the ascending sort button (↑)
            var sortButtons = cut.FindAll(".btn-arrow");
            var ascButton = sortButtons.First(b => b.TextContent == "↑");
            ascButton.Click();
            ascButton.Click();

            // Get all like counts
            var likeCounts = cut.FindAll(".like-count")
                .Select(e => int.Parse(e.TextContent))
                .ToList();

            // Assert - Check if the likes are in ascending order
            Assert.That(likeCounts, Is.Not.Ordered);
        }

        [Test]
        public void Sort_Descending_Should_Order_Products_By_Likes()
        {
            // Arrange
            var cut = RenderComponent<ProductList>();

            // Act - Find and click the descending sort button (↓)
            var sortButtons = cut.FindAll(".btn-arrow");
            var descButton = sortButtons.First(b => b.TextContent == "↓");
            descButton.Click();

            // Get all like counts
            var likeCounts = cut.FindAll(".like-count")
                .Select(e => int.Parse(e.TextContent))
                .ToList();

            // Assert - Check if the likes are in descending order
            Assert.That(likeCounts, Is.Ordered.Descending);
        }

        [Test]
        public void AddLike_Should_Increment_Product_Likes()
        {
            // Arrange
            _mockProductService.Setup(s => s.AddLike(It.IsAny<string>()))
                .Callback<string>(id =>
                {
                    var product = _testProducts.First(p => p.Id == id);
                    product.Likes++;
                });

            var cut = RenderComponent<ProductList>();

            // Get initial likes count for first product
            var initialLikes = int.Parse(cut.Find(".like-count").TextContent);

            // Act - Click the like button
            cut.Find(".like-button").Click();

            // Assert
            var newLikes = int.Parse(cut.Find(".like-count").TextContent);
            Assert.That(newLikes, Is.EqualTo(initialLikes + 1));
        }

        [Test]
        public void AddComment_Should_Add_Comment_To_Product()
        {
            // Arrange
            var testComment = "Great product!";

            var firstProduct = _testProducts[0];

            _mockProductService.Setup(s => s.GetAllData())
                .Returns(_testProducts);

            Services.AddSingleton<JsonFileProductService>(_mockProductService.Object);

            var cut = RenderComponent<ProductList>();

            // Act
            // Find and fill the comment input
            var commentInput = cut.Find(".comment-input");
            commentInput.Change(testComment);

            // Click the submit button
            cut.Find(".submit-comment-btn").Click();

            
        }

        [Test]
        public void FuzzySearch_Should_Return_Matching_Products()
        {
            // Arrange
            var cut = RenderComponent<ProductList>();
            var searchTerm = "mini";  // Should match "Minimalist White City Backpack"

            // Act
            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");
            searchInput.Input(searchTerm);
            searchInput.KeyDown("Enter");

            // Assert
            var result = cut.Markup;

            // Woven-tote shows up in trending products too, we want to see if there is now one occurance instead of two
            var tote_occurances = result.Split("woven-tote").Length - 1;

            var backpack_occurances = result.Split("Minimalist White City Backpack").Length - 1;

            // Assert
            Assert.That(tote_occurances, Is.LessThan(2));
            Assert.That(backpack_occurances, Is.EqualTo(3));
        }

        [Test]
        public void EmptySearch_Should_Show_All_Products()
        {
            // Arrange
            var cut = RenderComponent<ProductList>();

            // Act
            var searchInput = cut.Find("input[placeholder='Search (Powered By Buzzword)']");
            searchInput.Input("");
            searchInput.KeyDown("Enter");

            // Assert
            var result = cut.Markup;
            Assert.That(result, Does.Contain("Minimalist White City Backpack"));
            Assert.That(result, Does.Contain("Woven Tote"));
        }
    }

}
