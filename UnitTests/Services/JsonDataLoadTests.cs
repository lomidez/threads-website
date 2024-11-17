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
    public class JsonDataLoadTests
    {
        private JsonFileProductService _productService;
        private Mock<IWebHostEnvironment> _mockEnvironment;

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





        [Test]
        public void GetAllData_Valid_Should_Load_Successfully()
        {
            // Act
            var products = _productService.GetAllData();

            // Assert
            Assert.That(products, Is.Not.Null, "Product list should not be null.");
            Assert.That(products.Any(), Is.True, "Product list should contain data.");
        }





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
