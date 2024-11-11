using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace UnitTests.Services
{
    /// <summary>
    /// Unit test class for testing the data loading functionality of JsonFileProductService.
    /// </summary>
    public class JsonDataLoadTests
    {
        // Instance of the JsonFileProductService being tested.
        private JsonFileProductService _productService;

        // Mock object for IWebHostEnvironment to simulate the environment.
        private Mock<IWebHostEnvironment> _mockEnvironment;

        /// <summary>
        /// Initializes the test environment and sets up the mocked services before each test.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Arrange: Set up a mock environment to provide necessary file paths.
            _mockEnvironment = new Mock<IWebHostEnvironment>();

            // Mock the WebRootPath to return the current directory.
            _mockEnvironment.Setup(m => m.WebRootPath).Returns(Directory.GetCurrentDirectory());

            // Mock the environment name to specify it's a unit test environment.
            _mockEnvironment.Setup(m => m.EnvironmentName).Returns("UnitTests");

            // Initialize JsonFileProductService with the mocked IWebHostEnvironment.
            _productService = new JsonFileProductService(_mockEnvironment.Object);
        }

        /// <summary>
        /// Test to verify that valid JSON data is loaded successfully by the GetAllData method.
        /// </summary>
        [Test]
        public void JsonData_Valid_Should_Load_Successfully()
        {
            // Act: Call GetAllData() to retrieve the product list from JSON file.
            var products = _productService.GetAllData();

            // Assert: Verify that the product list is not null and contains data.
            Assert.That(products, Is.Not.Null, "Product list should not be null.");
            Assert.That(products.Any(), Is.True, "Product list should contain data.");
        }

        /// <summary>
        /// Test to check that the loaded product data contains at least one entry.
        /// </summary>
        [Test]
        public void ProductData_Valid_Should_Contain_At_Least_One_Entry()
        {
            // Act: Retrieve all products using GetAllData().
            var products = _productService.GetAllData();

            // Assert: Ensure that the product list has at least one entry.
            Assert.That(products.Count, Is.GreaterThan(0), "Product list should contain at least one product.");
        }
    }
}
