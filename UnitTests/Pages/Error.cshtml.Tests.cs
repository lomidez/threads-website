using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Pages;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Pages.Error
{
    /// <summary>
    /// Unit test class for testing the ErrorModel page functionality.
    /// </summary>
    public class ErrorTests
    {
        #region TestSetup

        // Instance of the ErrorModel page being tested.
        public static ErrorModel pageModel;

        /// <summary>
        /// Initializes the test setup by creating mock services and setting up the ErrorModel instance.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Create a mock logger for ErrorModel using Moq's Mock.Of() for simplicity.
            var MockLoggerDirect = Mock.Of<ILogger<ErrorModel>>();

            // Initialize the ErrorModel with the mocked logger and test context.
            pageModel = new ErrorModel(MockLoggerDirect)
            {
                // Assign test helpers for PageContext and TempData.
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
            };
        }

        #endregion TestSetup

        #region OnGet

        /// <summary>
        /// Test to verify that the OnGet method sets a valid RequestId when an activity is active.
        /// </summary>
        [Test]
        public void OnGet_Valid_Activity_Set_Should_Return_RequestId()
        {
            // Arrange - Start a new Activity with a specified name.
            Activity activity = new Activity("activity");
            activity.Start();

            // Act - Call the OnGet method to simulate a GET request.
            pageModel.OnGet();

            // Reset - Stop the activity after the test.
            activity.Stop();

            // Assert - Verify that the ModelState is valid and the RequestId matches the activity's Id.
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(pageModel.RequestId, Is.EqualTo(activity.Id));
        }

        /// <summary>
        /// Test to verify that the OnGet method sets the RequestId to the trace identifier
        /// when there is no active activity.
        /// </summary>
        [Test]
        public void OnGet_InValid_Activity_Null_Should_Return_TraceIdentifier()
        {
            // Arrange - No specific arrangement needed for this test.

            // Act - Call the OnGet method without starting an activity.
            pageModel.OnGet();

            // Reset - No reset actions needed for this test.

            // Assert - Verify that the ModelState is valid, the RequestId is set to "trace", and ShowRequestId is true.
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(pageModel.RequestId, Is.EqualTo("trace"));
            Assert.That(pageModel.ShowRequestId, Is.EqualTo(true));
        }

        #endregion OnGet
    }
}
