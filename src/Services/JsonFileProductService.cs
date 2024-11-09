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
    /// Service class for managing product data using a JSON file.
    /// </summary>
    public class JsonFileProductService
    {
        // Holds mock product data for testing purposes
        private List<ProductModel> _testProducts;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileProductService"/> class.
        /// </summary>
        /// <param name="webHostEnvironment">The web host environment for accessing the web root path.</param>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonFileProductService"/> class with mock data for testing.
        /// </summary>
        /// <param name="webHostEnvironment">The web host environment for accessing the web root path.</param>
        /// <param name="testProducts">The mock product data for testing.</param>
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment, List<ProductModel> testProducts)
        {
            WebHostEnvironment = webHostEnvironment;
            _testProducts = testProducts; // Assign test data
        }

        /// <summary>
        /// Gets the web host environment.
        /// </summary>
        public IWebHostEnvironment WebHostEnvironment { get; }

        /// <summary>
        /// Gets the file name for the JSON file containing product data.
        /// </summary>
        private string JsonFileName
        {
            get
            {
                // Find the path relative to the project root for consistent access during tests
                return Path.Combine(WebHostEnvironment.WebRootPath, "data", "products.json");
            }
        }

        /// <summary>
        /// Gets all product data. Returns mock data if available, otherwise reads from the JSON file.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="ProductModel"/> objects.</returns>
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
        /// Adds a rating to a product by its ID.
        /// </summary>
        /// <param name="productId">The product ID to which the rating is added.</param>
        /// <param name="rating">The rating value (0-5).</param>
        /// <returns>True if the rating was added successfully; otherwise, false.</returns>
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

            // Initialize ratings array if it's null
            if (data.Ratings == null)
            {
                data.Ratings = new int[] { };
            }

            var ratings = data.Ratings.ToList(); // Convert existing ratings to a list
            ratings.Add(rating); // Add the new rating
            data.Ratings = ratings.ToArray(); // Assign the updated ratings array

            SaveData(products);

            return true;
        }

        /// <summary>
        /// Updates the data of an existing product.
        /// </summary>
        /// <param name="data">The updated product data.</param>
        /// <returns>The updated <see cref="ProductModel"/> if the product exists; otherwise, null.</returns>
        public virtual ProductModel UpdateData(ProductModel data)
        {
            var products = GetAllData().ToList();
            var productData = products.FirstOrDefault(x => x.Id.Equals(data.Id));

            // Return null if the product doesn't exist
            if (productData == null)
            {
                return null;
            }

            // Update product fields with new data
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
        /// Saves the provided list of products to the JSON file.
        /// </summary>
        /// <param name="products">The list of products to save.</param>
        public void SaveData(IEnumerable<ProductModel> products)
        {
            try
            {
                using (var outputStream = File.Create(JsonFileName))
                {
                    JsonSerializer.Serialize(
                        new Utf8JsonWriter(
                            outputStream, 
                            new JsonWriterOptions { 
                                SkipValidation = true, 
                                Indented = true 
                            }),
                        products);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a new product entry and saves it to the data store.
        /// </summary>
        /// <param name="data">The product data to create.</param>
        /// <returns>True if the product was created successfully; otherwise, false.</returns>
        public virtual bool CreateData(ProductModel data)
        {
            try
            {
                var dataSet = GetAllData().Append(data);
                SaveData(dataSet);
                return true; // Return true if the operation succeeds
            }
            catch
            {
                // Log the error if necessary
                return false; // Return false if an error occurs during saving
            }
        }

        /// <summary>
        /// Increments the like count for a specific product.
        /// </summary>
        /// <param name="productId">The product ID whose like count is to be incremented.</param>
        /// <returns>True if the like count was incremented successfully; otherwise, false.</returns>
        public bool AddLike(string productId)
        {
            var products = GetAllData().ToList();
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return false;

            product.Likes++;
            SaveData(products);
            return true;
        }

        /// <summary>
        /// Resets the like count for a specific product to zero.
        /// </summary>
        /// <param name="productId">The product ID whose like count is to be reset.</param>
        /// <returns> True if the like count was reset successfully ; otherwise False</returns>
        public bool ResetLikes(string productId)
        {
            var products = GetAllData().ToList();
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return false;

            product.Likes = 0;
            SaveData(products);
            return true;
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id"> the product Id to delete </param>
        /// <returns> The deleted <see cref="ProductModel"/> if the product was found and deleted; otherwise null </returns>
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

        public Dictionary<string, HashSet<string>> GetAllDataPermutations()
        {
            var products = GetAllData();

            return new Dictionary<string, HashSet<string>>
            {
                ["Categories"] = new HashSet<string>(products.Select(p => p.Category)),
                ["Sizes"] = new HashSet<string>(products.Select(p => p.Size)),
                ["Colors"] = new HashSet<string>(products.Select(p => p.Color)),
            };
        }
    }
}
