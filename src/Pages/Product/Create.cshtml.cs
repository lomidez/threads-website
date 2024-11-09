using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// The CreateModel class handles the creation of a new product.
    /// Provides methods for form submission, product validation, and duplicate ID checks.
    /// </summary>
    public class CreateModel : PageModel
    {
        // Service for handling JSON file operations for products
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Constructor for CreateModel. Initializes the product service.
        /// </summary>
        /// <param name="productService">The product service to manage product data.</param>
        public CreateModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Represents the new product to be created, bound to the form input.
        /// </summary>
        [BindProperty]
        public ProductModel NewProduct { get; set; } = new ProductModel();

        /// <summary>
        /// Handles the form submission for creating a new product.
        /// Validates product ID, checks for duplicates, and saves the product if valid.
        /// </summary>
        /// <returns>Redirects to the Index page if successful, or returns the current page if validation fails.</returns>
        public IActionResult OnPost()
        {
            // Validate the Product ID format
            if (!ValidateProductId(NewProduct.Id))
            {
                return Page(); // Return to the form if validation fails
            }

            // Check if the Product ID already exists
            if (IsDuplicateProductId(NewProduct.Id))
            {
                // Add error message for duplicate ID
                ModelState.AddModelError("NewProduct.Id", "This ID already exists. Please select a new ID.");
                return Page();
            }

            // Proceed if ModelState is valid
            if (ModelState.IsValid)
            {
                // Attempt to save the new product data
                var success = _productService.CreateData(NewProduct);
                if (!success)
                {
                    // Add error message if saving fails
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the product. Please try again.");
                    return Page();
                }

                // Display a success notification using TempData
                TempData["Notification"] = "Product successfully created.";
                return RedirectToPage("./Index"); // Redirect to Index page upon success
            }

            return Page(); // Return the form if ModelState is invalid
        }

        /// <summary>
        /// Validates the format of the product ID.
        /// Ensures the ID is not empty and follows the required pattern.
        /// </summary>
        /// <param name="productId">The product ID to validate.</param>
        /// <returns>True if the product ID is valid; otherwise, false.</returns>
        private bool ValidateProductId(string productId)
        {
            // Check if the product ID is null, empty, or whitespace
            if (string.IsNullOrWhiteSpace(productId))
            {
                ModelState.AddModelError("NewProduct.Id", "Product ID cannot be empty.");
                return false;
            }

            // Check if the product ID contains only alphanumeric characters and hyphens
            if (!Regex.IsMatch(productId, @"^[a-zA-Z0-9-]+$"))
            {
                ModelState.AddModelError("NewProduct.Id", "Product ID format is invalid. Use only alphanumeric characters and hyphens.");
                return false;
            }

            return true; // Product ID is valid
        }

        /// <summary>
        /// Checks if the provided product ID already exists in the data store.
        /// </summary>
        /// <param name="productId">The product ID to check for duplication.</param>
        /// <returns>True if the product ID is a duplicate; otherwise, false.</returns>
        private bool IsDuplicateProductId(string productId)
        {
            // Retrieve all existing products from the data service
            var products = _productService.GetAllData();

            // Check if any product has the same ID
            return products.Any(p => p.Id == productId);
        }
    }
}
