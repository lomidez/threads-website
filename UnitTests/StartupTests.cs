using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;

namespace UnitTests.Pages.Startup
{
    /// <summary>
    /// Unit tests for the <see cref="ContosoCrafts.WebSite.Startup"/> class.
    /// </summary>
    public class StartupTests
    {
        #region TestSetup

        /// <summary>
        /// Initializes the test setup before each test run.
        /// This method is called before each test method is executed.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Setup logic (if needed) for each test can be added here.
        }

        /// <summary>
        /// Custom implementation of the <see cref="ContosoCrafts.WebSite.Startup"/> class
        /// for testing purposes. It passes the configuration to the base constructor.
        /// </summary>
        public class Startup : ContosoCrafts.WebSite.Startup
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Startup"/> class.
            /// </summary>
            /// <param name="config">The configuration to be passed to the base <see cref="Startup"/> class.</param>
            public Startup(IConfiguration config) : base(config) { }
        }
        #endregion TestSetup

        #region ConfigureServices

        /// <summary>
        /// Test for the <see cref="Startup.ConfigureServices"/> method.
        /// This test ensures that the method executes without any issues using the default configuration.
        /// </summary>
        [Test]
        public void Startup_ConfigureServices_Valid_Defaut_Should_Pass()
        {
            // Arrange: Create a web host using the Startup class and default configuration.
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();

            // Assert: Ensure that the web host is not null after building.
            Assert.That(webHost, Is.Not.Null);
        }
        #endregion ConfigureServices

        #region Configure

        /// <summary>
        /// Test for the <see cref="Startup.Configure"/> method.
        /// This test ensures that the method executes without any issues using the default configuration.
        /// </summary>
        [Test]
        public void Startup_Configure_Valid_Defaut_Should_Pass()
        {
            // Arrange: Create a web host using the Startup class and default configuration.
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();

            // Assert: Ensure that the web host is not null after building.
            Assert.That(webHost, Is.Not.Null);
        }

        #endregion Configure
    }
}
