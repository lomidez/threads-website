using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ContosoCrafts.WebSite.Pages
{
    /// <summary>
    /// Represents the model for the Read page, inheriting from PageModel.
    /// This class handles the retrieval and display of a selected product's details.
    /// </summary>
    public class ReadModel : PageModel
    {
        // The service used for accessing product data from the JSON file
        private readonly JsonFileProductService _productService;

        /// <summary>
        /// Initializes a new instance of the ReadModel class with the specified product service.
        /// </summary>
        /// <param name="productService">The service used to retrieve product data in JSON format.</param>
        public ReadModel(JsonFileProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Gets or sets the currently selected product to be displayed on the page.
        /// </summary>
        public ProductModel SelectedProduct { get; set; }

        /// <summary>
        /// Handles HTTP GET requests to display information about a specific product.
        /// Retrieves the product based on the provided ID parameter.
        /// </summary>
        /// <param name="id">The ID of the product to retrieve.</param>
        public void OnGet(string id)
        {
            // Retrieve the product with the matching ID from the product data
            SelectedProduct = _productService.GetAllData().FirstOrDefault(p => p.Id == id);
        }
    }
}
