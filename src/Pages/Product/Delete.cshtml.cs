using ContosoCrafts.WebSite.Models;
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
            if (SelectedProduct == null || string.IsNullOrWhiteSpace(SelectedProduct.Id))
            {
                TempData["Notification"] = "Error: Failed to delete product.";
                return NotFound();
            }

            // Attempt to delete the product directly using the ID from SelectedProduct
            var deletedProduct = _productService.DeleteData(SelectedProduct.Id);
            if (deletedProduct == null)
            {
                TempData["Notification"] = "Error: Failed to delete product.";
                return NotFound();
            }

            TempData["Notification"] = "Product successfully deleted.";
            return RedirectToPage("./Index");
        }









    }
}
