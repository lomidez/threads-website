using Microsoft.Extensions.Logging;

using NUnit.Framework;

using Moq;

using ContosoCrafts.WebSite.Pages;


namespace UnitTests.Pages.Privacy
{
    /// <summary>
    /// Unit test class for testing the PrivacyModel page functionality.
    /// </summary>
    public class PrivacyTests
    {
        /// <summary>
        /// Initializes the test setup by creating mock services and setting up the PrivacyModel instance.
        /// </summary>
        #region TestSetup
        public static PrivacyModel pageModel;

        [SetUp]
        public void TestInitialize()
        {
            var MockLoggerDirect = Mock.Of<ILogger<PrivacyModel>>();

            pageModel = new PrivacyModel(MockLoggerDirect)
            {
                PageContext = TestHelper.PageContext,
                TempData = TestHelper.TempData,
            };
        }

        #endregion TestSetup

        /// <summary>
        /// Test to verify that the OnGet method initializes the PrivacyModel correctly
        /// and that the ModelState is valid after the method execution.
        /// </summary>
        #region OnGet
        [Test]
        public void OnGet_Valid_Activity_Set_Should_Return_RequestId()
        {
            // Arrange

            // Act
            pageModel.OnGet();

            // Reset

            // Assert
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
        }

        #endregion OnGet
    }
}