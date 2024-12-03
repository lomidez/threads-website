using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// The DeleteModel class handles the deletion of a product.
    /// Provides methods for fetching product details and deleting the selected product.
    /// </summary>
    public class DeleteModel : PageModel
    {
        // Service for handling JSON file operations for products
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Constructor for DeleteModel. Initializes the product service.
        /// </summary>
        /// <param name="productService">The product service to manage product data.</param>
        public DeleteModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Represents the product selected for deletion, bound to the form input.
        /// </summary>
        [BindProperty]
        public ProductModel SelectedProduct { get; set; }

        /// <summary>
        /// Holds the material details of the selected product, bound to the form input.
        /// </summary>
        [BindProperty]
        public string Material { get; set; }

        /// <summary>
        /// Holds the style details of the selected product, bound to the form input.
        /// </summary>
        [BindProperty]
        public string Style { get; set; }

        /// <summary>
        /// Handles GET requests to fetch and display the details of the product to be deleted.
        /// </summary>
        /// <param name="id">The ID of the product to fetch.</param>
        public void OnGet(string id)
        {
            // Retrieve the selected product based on the provided ID
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);

            // If the product is not found, return immediately or handle accordingly
            if (SelectedProduct == null)
            {
                return;
            }

            // Join material list as a comma-separated string
            Material = string.Join(", ", SelectedProduct.Material ?? new List<string>());

            // Join style list as a comma-separated string
            Style = string.Join(", ", SelectedProduct.Style ?? new List<string>());
        }

        /// <summary>
        /// Handles POST requests to delete the selected product.
        /// Validates the product and attempts to delete it from the data store.
        /// </summary>
        /// <returns>Redirects to the Index page if successful, or returns a NotFound result if deletion fails.</returns>
        public IActionResult OnPost()
        {
            // Check if the selected product is null or has an invalid ID
            if (SelectedProduct == null)
            {
                // Set notification message for deletion error
                TempData["Notification"] = "Error: Failed to delete product, product does not exist.";
                return NotFound(); // Return 404 error if product is invalid
            }

            if (string.IsNullOrWhiteSpace(SelectedProduct.Id))
            {
                // Set notification message for deletion error
                TempData["Notification"] = "Error: Failed to delete product, product could not be located.";
                return NotFound(); // Return 404 error if product is invalid
            }

            // Attempt to delete the product using the ID from SelectedProduct
            var deletedProduct = _productService.DeleteData(SelectedProduct.Id);
            if (deletedProduct == null)
            {
                // Set notification message if deletion fails
                // TempData["Notification"] = "Error: Failed to delete product.";
                // return NotFound(); // Return 404 error if deletion is unsuccessful
                return NotFound();
            }  
            return RedirectToPage("./Index"); // Redirect after deletion          
        }
    }
}
