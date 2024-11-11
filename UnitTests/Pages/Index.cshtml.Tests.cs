using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ContosoCrafts.WebSite.Pages;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Index
{
    /// <summary>
    /// Unit test class for testing the IndexModel page functionality.
    /// </summary>
    public class IndexTests
    {
        #region TestSetup

        // Instance of the IndexModel page being tested.
        public static IndexModel pageModel;

        /// <summary>
        /// Initializes the test setup by creating mock services and setting up the IndexModel instance.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Create a mock logger for IndexModel using Moq's Mock.Of() for simplicity.
            var MockLoggerDirect = Mock.Of<ILogger<IndexModel>>();

            // Initialize the IndexModel with the mocked logger and the test ProductService.
            pageModel = new IndexModel(MockLoggerDirect, TestHelper.ProductService);
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Test to verify that the OnGet method returns a list of products when called with a valid state.
        /// </summary>
        [Test]
        public void OnGet_Valid_Should_Return_Products()
        {
            // Arrange - No specific setup needed for this test.

            // Act - Call the OnGet method with a null search term to load all products.
            pageModel.OnGet(null);

            // Assert - Verify that the ModelState is valid and that the Products list is not empty.
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(pageModel.Products.ToList().Any(), Is.EqualTo(true));
        }

        #endregion OnGet
    }
}
