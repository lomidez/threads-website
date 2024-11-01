using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ContosoCrafts.WebSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace ContosoCrafts.WebSite.Services
{
    /// <summary>
    /// Service class for handling CRUD operations and ratings for products
    /// stored in a JSON file.
    /// </summary>
    public class JsonFileProductService
    {
        // Field for holding mock data during tests
        private List<ProductModel> _testProducts; 

        /// <summary>
        /// Initializes a new instance of the JsonFileProductService class without mock data.
        /// </summary>
        /// <param name="webHostEnvironment">Provides information about the web hosting environment.</param>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Initializes a new instance of the JsonFileProductService class with mock data for testing.
        /// </summary>
        /// <param name="webHostEnvironment">Provides information about the web hosting environment.</param>
        /// <param name="testProducts">List of test products for mock data during testing.</param>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment, List<ProductModel> testProducts)
        {
            WebHostEnvironment = webHostEnvironment;
            _testProducts = testProducts; // Assign test data
        }

        /// <summary>
        /// Provides access to the web hosting environment.
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// Gets the file path of the products JSON file.
        /// </summary>
        private string JsonFileName
        {
            get
            {
                // Find the path relative to the project root for consistent access during tests
                return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "products.json");
            }
        }

        /// <summary>
        /// Retrieves all product data from the JSON file or returns mock data if provided.
        /// </summary>
        /// <returns>Collection of ProductModel representing all products.</returns>
        public virtual IEnumerable<ProductModel> GetAllData()
        {
            // Return mock data if provided for testing
            if (_testProducts != null)
            {
                return _testProducts;
            }

            // Otherwise, load from products.json file
            using (var jsonFileReader = File.OpenText(JsonFileName))
            {
                return JsonSerializer.Deserialize<ProductModel[]>(jsonFileReader.ReadToEnd(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }

        /// <summary>
        /// Adds a rating to the specified product.
        /// </summary>
        /// <param name="productId">The ID of the product to add a rating to.</param>
        /// <param name="rating">The rating value, expected between 0 and 5.</param>
        /// <returns>True if the rating was added successfully, otherwise false.</returns>
        public bool AddRating(string productId, int rating)
        {
            if (string.IsNullOrEmpty(productId) || rating < 0 || rating > 5)
            {
                return false;
            }

            var products = GetAllData().ToList();
            var data = products.FirstOrDefault(x => x.Id.Equals(productId));
            if (data == null)
            {
                return false;
            }

            if (data.Ratings == null)
            {
                data.Ratings = new int[] { };
            }

            var ratings = data.Ratings.ToList();
            ratings.Add(rating);
            data.Ratings = ratings.ToArray();

            SaveData(products);

            return true;
        }

        /// <summary>
        /// Updates an existing product with the provided data.
        /// </summary>
        /// <param name="data">ProductModel with updated data.</param>
        /// <returns>The updated ProductModel if successful, otherwise null.</returns>
        public virtual ProductModel UpdateData(ProductModel data)
        {
            var products = GetAllData().ToList();
            var productData = products.FirstOrDefault(x => x.Id.Equals(data.Id));

            // Return null if the product doesn't exist
            if (productData == null)
            {
                return null;
            }

            // Update fields
            productData.Title = data.Title;
            productData.Description = data.Description?.Trim();
            productData.Url = data.Url;
            productData.Image = data.Image;
            productData.Quantity = data.Quantity;
            productData.Price = data.Price;
            productData.Size = data.Size;
            productData.Color = data.Color;
            productData.Material = data.Material;
            productData.Style = data.Style;
            productData.CommentList = data.CommentList;

            SaveData(products);
            return productData;
        }

        /// <summary>
        /// Saves the provided collection of products to the JSON file.
        /// </summary>
        /// <param name="products">Collection of ProductModel to be saved.</param>
        public void SaveData(IEnumerable<ProductModel> products)
        {
            using (var outputStream = File.Create(JsonFileName))
            {
                JsonSerializer.Serialize<IEnumerable<ProductModel>>(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    products
                );
            }
        }

        /// <summary>
        /// Creates a new product entry.
        /// </summary>
        /// <param name="data">ProductModel with new product data.</param>
        /// <returns>The created ProductModel.</returns>
        public ProductModel CreateData(ProductModel data)
        {
            var dataSet = GetAllData().Append(data);

            SaveData(dataSet);

            return data;
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id">The ID of the product to delete.</param>
        /// <returns>The deleted ProductModel if successful, otherwise null.</returns>
        public virtual ProductModel DeleteData(string id)
        {
            var productList = _testProducts ?? GetAllData().ToList(); // Use _testProducts if in test mode

            var productToDelete = productList.FirstOrDefault(m => m.Id == id);

            if (productToDelete != null)
            {
                productList.Remove(productToDelete);
                SaveData(productList);
                return productToDelete;
            }

            return null;
        }
    }
}
