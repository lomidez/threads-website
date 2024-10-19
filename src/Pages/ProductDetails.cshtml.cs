using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly JsonFileProductService _productService;

        public ProductDetailsModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        public ProductModel SelectedProduct { get; set; }

        public void OnGet(string id)
        {
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);
        }
    }
}