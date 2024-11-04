﻿using NUnit.Framework;
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
            // Mock the environment to provide the root path
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _mockEnvironment.Setup(m => m.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _mockEnvironment.Setup(m => m.EnvironmentName).Returns("UnitTests");

            // Initialize the service with the mock environment
            _productService = new JsonFileProductService(_mockEnvironment.Object);
        }

        [Test]
        public void JsonData_Should_Load_Successfully()
        {
            // Act
            var products = _productService.GetAllData();

            // Assert
            Assert.That(products, Is.Not.Null, "Product list should not be null.");
            Assert.That(products.Any(), Is.True, "Product list should contain data.");
        }





        [Test]
        public void ProductData_Should_Contain_At_Least_One_Entry()
        {
            // Act
            var products = _productService.GetAllData();

            // Assert
            Assert.That(products.Count, Is.GreaterThan(0), "Product list should contain at least one product.");
        }
    }
}
