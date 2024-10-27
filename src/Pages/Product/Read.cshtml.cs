using ContosoCrafts.WebSite.Models; 
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    ///  Represents the model for the product, inheriting from PageMode
    ///  This model handles the Read page's data and operations
    /// </summary>
    public class ReadModel : PageModel
    {
        // Initializes a variable of the product service
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Initializes a new instsance of ReadModel class
        /// </summary>
        /// <param name="productService">The service used to retrieve product data in JSON format</param>
        public ReadModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets or setes the currently selected product to be displayed on the page
        /// </summary>
        public ProductModel SelectedProduct { get; set; }

        /// <summary>
        /// Handles HTTP GET requests to display product informatiion
        /// </summary>
        /// <param name="id"></param>
        public void OnGet(string id)
        {
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);
        }
    }
}
