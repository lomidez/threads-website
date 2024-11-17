using NUnit.Framework;
using Moq;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnitTests.Services
{
    /// <summary>
    /// Unit tests for the JsonFileProductService class to validate the functionality of loading product data.
    /// </summary>
    public class JsonDataLoadTests
    {
        private JsonFileProductService _productService; ///<summary>Instance of JsonFileProductService used in the tests.</summary>
        private Mock<IWebHostEnvironment> _mockEnvironment; ///<summary>Mock of IWebHostEnvironment to simulate project environment for loading data.</summary>

        /// <summary>
        /// Initializes the test environment by mocking the IWebHostEnvironment and setting up the path to "src/wwwroot/data".
        /// This method is called before each test to set up necessary dependencies.
        /// </summary>
        [SetUp]
        public void TestInitialize()
        {
            // Mock environment setup with path to "src/wwwroot/data" in the project directory
            _mockEnvironment = new Mock<IWebHostEnvironment>();

            // Calculate the project root and append the relative path to wwwroot/data
            var projectDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../src/wwwroot"));
            _mockEnvironment.Setup(m => m.WebRootPath).Returns(projectDirectory);
            _mockEnvironment.Setup(m => m.EnvironmentName).Returns("UnitTests");

            // Initialize JsonFileProductService with mock environment
            _productService = new JsonFileProductService(_mockEnvironment.Object);
        }

        /// <summary>
        /// Test to verify that the GetAllData method successfully loads product data.
        /// </summary>
        /// <remarks>
        /// Verifies that calling GetAllData does not return a null value and the product list is populated with data.
        /// </remarks>
        [Test]
        public void GetAllData_Valid_Should_Load_Successfully()
        {
            // Act
            var products = _productService.GetAllData();

            // Assert
            Assert.That(products, Is.Not.Null, "Product list should not be null.");
            Assert.That(products.Any(), Is.True, "Product list should contain data.");
        }

        /// <summary>
        /// Test to verify that the GetAllData method returns a list that contains at least one product.
        /// </summary>
        /// <remarks>
        /// Verifies that the product list has at least one entry, indicating the data has been loaded correctly.
        /// </remarks>
        [Test]
        public void GetAllData_Valid_ProductData_Should_Contain_At_Least_One_Entry()
        {
            // Act
            var products = _productService.GetAllData();

            // Assert
            Assert.That(products.Count, Is.GreaterThan(0), "Product list should contain at least one product.");
        }
    }
}
