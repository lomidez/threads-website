using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Bunit;
using NUnit.Framework;
using ContosoCrafts.WebSite.Components;
using ContosoCrafts.WebSite.Services;

namespace UnitTests.Components
{
    /// <summary>
    /// Unit tests for the ProductList component to verify UI content.
    /// </summary>
    public class ProductListTests : BunitTestContext
    {
        #region TestSetup

        /// <summary>
        /// Initializes any necessary resources or configurations before each test.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Setup code for each test will go here if needed
        }

        #endregion TestSetup

        /// <summary>
        /// Test to check if the ProductList component renders the expected content by default.
        /// </summary>
        [Test]
        public void ProductList_Valid_Default_Should_Return_Content()
        {
            // Arrange: Add the product service to the dependency injection container
            Services.AddSingleton<JsonFileProductService>(TestHelper.ProductService);

            // Act: Render the ProductList component and capture the rendered HTML markup
            var page = RenderComponent<ProductList>();

            // Get the rendered content as markup (HTML)
            var result = page.Markup;

            // Assert: Verify that the expected product name is in the markup
            Assert.That(result.Contains("Burgundy Leather Chic Handbag"), Is.EqualTo(true));
        }
    }
}
