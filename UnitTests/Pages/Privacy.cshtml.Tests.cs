using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Pages;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Privacy
{
    /// <summary>
    /// Unit test class for testing the PrivacyModel page functionality.
    /// </summary>
    public class PrivacyTests
    {
        #region TestSetup

        // Instance of the PrivacyModel page being tested.
        public static PrivacyModel pageModel;

        /// <summary>
        /// Initializes the test setup by creating mock services and setting up the PrivacyModel instance.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Create a mock logger for PrivacyModel using Moq's Mock.Of() for simplicity.
            var MockLoggerDirect = Mock.Of<ILogger<PrivacyModel>>();

            // Initialize the PrivacyModel with the mocked logger and test context.
            pageModel = new PrivacyModel(MockLoggerDirect)
            {
                // Assign test helpers for PageContext and TempData.
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
            };
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Test to verify that the OnGet method initializes the PrivacyModel correctly
        /// and that the ModelState is valid after the method execution.
        /// </summary>
        [Test]
        public void OnGet_Valid_Activity_Set_Should_Return_RequestId()
        {
            // Arrange - No specific arrangement needed for this test.

            // Act - Call the OnGet method to simulate a GET request.
            pageModel.OnGet();

            // Reset - No reset actions needed for this test.

            // Assert - Verify that the ModelState is valid after the OnGet method is called.
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
        }

        #endregion OnGet
    }
}
