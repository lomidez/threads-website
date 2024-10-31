﻿using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly JsonFileProductService _productService;

        public DeleteModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public ProductModel SelectedProduct { get; set; }

        [BindProperty]
        public string Material { get; set; }

        [BindProperty]
        public string Style { get; set; }

        public void OnGet(string id)
        {
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);

            if (SelectedProduct != null)
            {
                Material = string.Join(", ", SelectedProduct.Material ?? new List<string>());
                Style = string.Join(", ", SelectedProduct.Style ?? new List<string>());
            }
        }

        public IActionResult OnPost()
        {
            // Fetch the selected product using its ID
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == SelectedProduct.Id);

            // Attempt to delete the product
            var deletedProduct = _productService.DeleteData(SelectedProduct.Id); // Call to the delete method in your service

            // Check if the deletedProduct is null, indicating the product was not found
            if (deletedProduct == null) // If deletion failed (product not found)
            {
                return NotFound(); // Return NotFound if product does not exist
            }

            return RedirectToPage("./Index"); // Redirect to the index page after deletion
        }

    }
}