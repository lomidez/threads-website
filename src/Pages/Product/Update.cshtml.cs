using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet(string id)
        {
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedProduct = _productService.UpdateData(SelectedProduct);
            
            if(updatedProduct == null)
            {
                return NotFound();
            }

            return RedirectToPage("./Index");
        }
    }
}
