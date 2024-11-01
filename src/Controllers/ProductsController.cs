using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ContosoCrafts.WebSite.Models;
using ContosoCrafts.WebSite.Services;

namespace ContosoCrafts.WebSite.Controllers
{
    /// <summary>
    /// API Controller to handle product-related HTTP requests.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the ProductsController class with the specified product service.
        /// </summary>
        /// <param name="productService">Service for managing product data.</param>
        public ProductsController(JsonFileProductService productService)
        {
            // Assign the injected product service
            ProductService = productService; 
        }

        /// <summary>
        /// Service for accessing product data from a JSON file.
        /// </summary>
        public JsonFileProductService ProductService { get; }

        /// <summary>
        /// Retrieves all product data.
        /// </summary>
        /// <returns> Enumerable list of all products.</returns>
        [HttpGet]
        public IEnumerable<ProductModel> Get()
        {
            // Call service method to get and fetch product data
            return ProductService.GetAllData(); 
        }
    }
}
