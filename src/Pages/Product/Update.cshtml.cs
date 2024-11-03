using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    public class UpdateModel : PageModel
    {
        private readonly JsonFileProductService _productService;

        public UpdateModel(JsonFileProductService productService)
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            SelectedProduct.Material = Material?.Split(',').Select(m => m.Trim()).ToList() ?? new List<string>();
            SelectedProduct.Style = Style?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>();

            var updatedProduct = _productService.UpdateData(SelectedProduct);
            
            if(updatedProduct == null)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }

        // Method to reset likes
        public IActionResult OnPostResetLikes(string id)
        {
            _productService.ResetLikes(id);
            return RedirectToPage("/Product/Update", new { id = id });
        }
    }
}
