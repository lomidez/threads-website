using Bunit;
using NUnit.Framework;

namespace UnitTests
{
    /// <summary>
    /// Provides a test context used by bUnit for component testing.
    /// This abstract class sets up and tears down the bUnit <see cref="TestContext"/>
    /// for each test to ensure proper initialization and resource cleanup.
    /// </summary>
    public abstract class BunitTestContext : TestContextWrapper
    {
        /// <summary>
        /// Sets up the <see cref="TestContext"/> before each test.
        /// This method is executed before each test to initialize the bUnit context.
        /// </summary>
        [SetUp]
        public void Setup() => TestContext = new Bunit.TestContext();

        /// <summary>
        /// Tears down the <see cref="TestContext"/> after each test.
        /// This method is executed after each test to dispose of the context and free up system resources.
        /// </summary>
        [TearDown]
        public void TearDown() => TestContext.Dispose();
    }
}
