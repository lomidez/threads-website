using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Represents the model for the Product Details page.
    /// Handles displaying product details and managing user interactions such as adding likes.
    /// </summary>
    public class ProductDetailsModel : PageModel
    {
        // Service used to retrieve and manage product data in JSON format
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Initializes a new instance of the ProductDetailsModel class.
        /// </summary>
        /// <param name="productService">The product service used for accessing product data.</param>
        public ProductDetailsModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets or sets the currently selected product to display on the Product Details page.
        /// </summary>
        public ProductModel SelectedProduct { get; set; }

        /// <summary>
        /// Handles HTTP GET requests to display product details.
        /// </summary>
        /// <param name="id">The ID of the product to display.</param>
        public void OnGet(string id)
        {
            // Retrieve the product with the specified ID from the product service
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// Handles HTTP POST requests to add a like to the product.
        /// </summary>
        /// <param name="id">The ID of the product to add a like to.</param>
        /// <returns>A redirect to the Product Details page with the updated like count.</returns>
        public IActionResult OnPostAddLike(string id)
        {
            // Increments the like count for the specified product
            _productService.AddLike(id);

            // Redirects back to the Product Details page for the specified product
            return RedirectToPage("/ProductDetails", new { id = id });
        }
    }
}
