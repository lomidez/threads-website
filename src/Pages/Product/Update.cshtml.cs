using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Represents the model for the Update page, handling the data and operations
    /// for updating an existing product's details, including material and style.
    /// </summary>
    public class UpdateModel : PageModel
    {
        // Service for accessing and updating product data from the JSON file
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Initializes a new instance of the UpdateModel class with the specified product service.
        /// </summary>
        /// <param name="productService">The service used to interact with product data.</param>
        public UpdateModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets or sets the currently selected product to be updated.
        /// </summary>
        [BindProperty]
        public ProductModel SelectedProduct { get; set; }

        /// <summary>
        /// Gets or sets the material details for the selected product as a comma-separated string.
        /// </summary>
        [BindProperty]
        public string Material { get; set; }

        /// <summary>
        /// Gets or sets the style details for the selected product as a comma-separated string.
        /// </summary>
        [BindProperty]
        public string Style { get; set; }

        /// <summary>
        /// Handles HTTP GET requests to display the update form for a specific product.
        /// Retrieves the product based on the provided ID and initializes the Material and Style properties.
        /// </summary>
        /// <param name="id">The ID of the product to be updated.</param>
        public void OnGet(string id)
        {
            // Find the product with the matching ID from the product data
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);

            // If the product exists, initialize Material and Style properties as comma-separated strings
            if (SelectedProduct != null)
            {
                Material = string.Join(", ", SelectedProduct.Material ?? new List<string>());
                Style = string.Join(", ", SelectedProduct.Style ?? new List<string>());
            }
        }

        /// <summary>
        /// Handles HTTP POST requests to update the product data.
        /// Updates the Material and Style properties based on user input and saves the changes.
        /// </summary>
        /// <returns>Redirects to the Index page if successful; otherwise, returns the current page.</returns>
        public IActionResult OnPost()
        {
            // Check if the model state is valid before proceeding with the update
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Parse Material and Style input strings into lists, trimming whitespace
            SelectedProduct.Material = Material?.Split(',').Select(m => m.Trim()).ToList() ?? new List<string>();
            SelectedProduct.Style = Style?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>();

            // Update the product data using the service and check if the update was successful
            var updatedProduct = _productService.UpdateData(SelectedProduct);

            // If the product was not found or update failed, return a NotFound result
            if (updatedProduct == null)
            {
                return NotFound();
            }

            // Redirect to the Index page upon successful update
            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Resets the likes count for a specific product.
        /// </summary>
        /// <param name="id">The ID of the product for which to reset likes.</param>
        /// <returns>Redirects to the Update page for the specified product.</returns>
        public IActionResult OnPostResetLikes(string id)
        {
            // Call the service method to reset likes for the product with the specified ID
            _productService.ResetLikes(id);

            // Redirect back to the Update page for the same product
            return RedirectToPage("/Product/Update", new { id = id });
        }

        /// <summary>
        /// Resets the comments for a specific product.
        /// </summary>
        /// <param name="id">The ID of the product for which to reset comments.</param>
        /// <returns>Redirects to the Update page for the specified product.</returns>
        public IActionResult OnPostResetComments(string id)
        {
            // Retrieve the product using the service
            var product = _productService.GetAllData().FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // Clear the comments list
                product.Comments.Clear();

                // Save the updated product data
                _productService.UpdateProduct(product);
            }

            // Redirect back to the Update page for the same product
            return RedirectToPage("/Product/Update", new { id = id });
        }
    }
}
