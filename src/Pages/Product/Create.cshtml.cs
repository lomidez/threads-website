using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ContosoCrafts.WebSite.Pages
{
    public class CreateModel : PageModel
    {
        private readonly JsonFileProductService _productService;

        public CreateModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductModel NewProduct { get; set; } = new ProductModel();

        public IActionResult OnPost()
        {
            // Validate the Product ID
            if (!ValidateProductId(NewProduct.Id))
            {
                return Page();
            }

            // Check for duplicate ID
            if (IsDuplicateProductId(NewProduct.Id))
            {
                ModelState.AddModelError("NewProduct.Id", "This ID already exists. Please select a new ID.");
                return Page();
            }

            // If ModelState is valid, try to create the new product
            if (ModelState.IsValid)
            {
                var success = _productService.CreateData(NewProduct);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the product. Please try again.");
                    return Page();
                }
                return RedirectToPage("./Index");
            }

            return Page(); // Return the page if ModelState is invalid
        }


        // Helper method for product ID validation
        private bool ValidateProductId(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
            {
                ModelState.AddModelError("NewProduct.Id", "Product ID cannot be empty.");
                return false;
            }

            if (!Regex.IsMatch(productId, @"^[a-zA-Z0-9-]+$"))
            {
                ModelState.AddModelError("NewProduct.Id", "Product ID format is invalid. Use only alphanumeric characters and hyphens.");
                return false;
            }

            return true;
        }

        // Helper method to check for duplicate product IDs
        private bool IsDuplicateProductId(string productId)
        {
            var products = _productService.GetAllData();
            return products.Any(p => p.Id == productId);
        }
    }
}
