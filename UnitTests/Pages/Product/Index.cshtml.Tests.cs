
//edited file for Index unit Test to have 100%
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using NUnit.Framework;

using ContosoCrafts.WebSite.Pages.Product;
using ContosoCrafts.WebSite.Services;
using System.IO;
using ContosoCrafts.WebSite.Models;

namespace UnitTests.Pages.Product.Index
{
    public class IndexTests
    {
        #region TestSetup
        public static IUrlHelperFactory urlHelperFactory;
        public static DefaultHttpContext httpContextDefault;
        public static IWebHostEnvironment webHostEnvironment;
        public static ModelStateDictionary modelState;
        public static ActionContext actionContext;
        public static EmptyModelMetadataProvider modelMetadataProvider;
        public static ViewDataDictionary viewData;
        public static TempDataDictionary tempData;
        public static PageContext pageContext;

        public static IndexModel pageModel;

        [SetUp]
        public void TestInitialize()
        {
            httpContextDefault = new DefaultHttpContext();

            modelState = new ModelStateDictionary();
            actionContext = new ActionContext(httpContextDefault, httpContextDefault.GetRouteData(), new PageActionDescriptor(), modelState);

            modelMetadataProvider = new EmptyModelMetadataProvider();
            viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
            tempData = new TempDataDictionary(httpContextDefault, Mock.Of<ITempDataProvider>());

            pageContext = new PageContext(actionContext)
            {
                ViewData = viewData,
            };

            var mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            mockWebHostEnvironment.Setup(m => m.EnvironmentName).Returns("Hosting:UnitTestEnvironment");

            // Update WebRootPath to a path that GitHub users will have when cloning the repository
            mockWebHostEnvironment.Setup(m => m.WebRootPath).Returns(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));
            mockWebHostEnvironment.Setup(m => m.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            var MockLoggerDirect = Mock.Of<ILogger<IndexModel>>();
            JsonFileProductService productService;

            productService = new JsonFileProductService(mockWebHostEnvironment.Object);

            pageModel = new IndexModel(productService);
        }


        #endregion TestSetup

        #region OnGet
        [Test]
        public void OnGet_Valid_ProductList_Should_Return_ProductCount_Of_25()
        {
            // Arrange - Set up 25 products in the mock data source
            var mockProducts = Enumerable.Range(1, 25).Select(i => new ProductModel
            {
                Id = $"product_{i}",
                Title = $"Product {i}",
                Ratings = new int[] { 4, 5 }
            }).ToList();

            var mockEnvironment = new Mock<IWebHostEnvironment>();
            var productService = new JsonFileProductService(mockEnvironment.Object, mockProducts);

            pageModel = new IndexModel(productService);

            // Act
            pageModel.OnGet();

            // Assert
            Assert.That(pageModel.ModelState.IsValid, Is.EqualTo(true));
            Assert.That(pageModel.Products.ToList().Count, Is.EqualTo(25));
        }

        #endregion OnGet
    }
}