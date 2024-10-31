using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var products = _productService.GetAllData();

            if (products.Any(p => p.Id == NewProduct.Id))
            {
                ModelState.AddModelError("NewProduct.Id", "This ID already exists. Please select a new ID.");
                return Page();
            }

            _productService.CreateData(NewProduct);

            return RedirectToPage("./Index");
        }
    }
}
