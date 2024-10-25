using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Handles the logic for displaying products on the index page.
    /// </summary>
    /// <remarks>
    /// Authors: Lisa Lomidze, Isaac Yushaiyin, Jooa Lee, Jacobie Fullerton
    /// </remarks>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// Logger instance used to log information and errors for the IndexModel class.
        /// </summary>
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="productService">The service used to fetch product data.</param>
        public IndexModel(
            ILogger<IndexModel> logger,
            JsonFileProductService productService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ProductService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        /// <summary>
        /// Gets the product service used to fetch product data.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Gets the products to be displayed on the index page.
        /// </summary>
        public IEnumerable<ProductModel> Products { get; private set; }

        /// <summary>
        /// Handles the GET request for the index page and fetches all product data.
        /// Sets the Products property with the retrieved product data from the service.
        /// </summary>
        public void OnGet()
        {
            // Fetch all data and assign it to the Products property
            Products = ProductService.GetAllData();

        }
    }
}
