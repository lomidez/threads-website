using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using System;
using System.Linq;

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
            _logger = logger;
            ProductService = productService;
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
        /// Gets the tag used to filter products on the index page.
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// Handles the GET request for the index page and fetches all product data.
        /// If a tag is provided, it filters the products by the tag.
        /// </summary>
        /// <param name="tag">The tag to filter the products by (optional).</param>
        public void OnGet(string tag)
        {
            // Set the Tag property from the query parameter
            Tag = tag;

            // Fetch all data from the service
            var allProducts = ProductService.GetAllData();

            // Initialize Products to show all products by default
            Products = allProducts;

            // Filter products if a tag is provided
            if (!string.IsNullOrEmpty(Tag))
            {
                Products = allProducts.Where(p =>
                    p.Category.ToString().Equals(Tag, StringComparison.OrdinalIgnoreCase) ||
                    p.Size.ToString().Equals(Tag, StringComparison.OrdinalIgnoreCase) ||
                    p.Color.Equals(Tag, StringComparison.OrdinalIgnoreCase) ||
                    p.Material.Any(m => m.Equals(Tag, StringComparison.OrdinalIgnoreCase)) ||
                    p.Style.Any(s => s.Equals(Tag, StringComparison.OrdinalIgnoreCase))
                );
            }
        }
    }
}
