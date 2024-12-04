using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        /// Updates an existing product in the product list and saves the changes to the JSON file.
        /// If the product ID matches an existing product, the product is updated, and the data is persisted.
        /// </summary>
        /// <param name="product">The <see cref="ProductModel"/> containing the updated product details.</param>
        public void UpdateProduct(ProductModel product)
        {
            var products = GetAllData().ToList();
            var index = products.FindIndex(p => p.Id == product.Id);
            
            if (index == -1)
            {
                return;
            }
            products[index] = product;
            SaveData(products);
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
            if (_testProducts == null)
            {
                using (var jsonFileReader = File.OpenText(JsonFileName))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    return JsonSerializer.Deserialize<ProductModel[]>(jsonFileReader.ReadToEnd(),
                        options);
                }
            }
            return _testProducts;
        }

        /// <summary>
        /// Adds a rating to a product by its ID.
        /// </summary>
        /// <param name="productId">The product ID to which the rating is added.</param>
        /// <param name="rating">The rating value (0-5).</param>
        /// <returns>True if the rating was added successfully; otherwise, false.</returns>
        public virtual bool AddRating(string productId, int rating)
        {
            // Validates the input parameters
            if (string.IsNullOrEmpty(productId))
            {
                return false;
            }
            if (rating < 0)
            {
                return false;
            }

            if (rating > 5)
            {
                return false;
            }

            // Loads all products
            var products = GetAllData().ToList();
            // Finds product by ID
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

            // Convert existing ratings to a list
            var ratings = data.Ratings.ToList();
            // Add the new rating
            ratings.Add(rating);
            // Assign the updated ratings array
            data.Ratings = ratings.ToArray();

            // Saves updated product list
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
            // Loads all products
            var products = GetAllData().ToList();
            // Finds product by ID
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

            // Saves updated product list
            SaveData(products);
            return productData;
        }

        /// <summary>
        /// Saves the provided list of products to the JSON file.
        /// </summary>
        /// <param name="products">The list of products to save.</param>
        public virtual void SaveData(IEnumerable<ProductModel> products)
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
            // Validate the data object before proceeding
            if (data == null)
            {
                // Log error or take necessary action
                return false;
            }

            if (string.IsNullOrWhiteSpace(data.Title))
            {
                return false;
            }

            // Ensure the product has a unique ID if not set
            if (string.IsNullOrWhiteSpace(data.Id))
            {
                data.Id = Guid.NewGuid().ToString("N");
            }

            // Get existing data
            var existingData = GetAllData();
           

            // Add the new product to the existing data
            var dataSet = existingData.ToList();
            dataSet.Add(data);

            // Save the data and ensure it succeeds
            return SaveDataSafely(dataSet);
        }

        public bool SaveDataSafely(IEnumerable<ProductModel> dataSet)
        {
            if (dataSet == null)
            {
                return false; // Safeguard against null data sets
            }

            // Use a validation approach to ensure safe saving
            try
            {
                SaveData(dataSet);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                Console.WriteLine($"Error saving data: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Increments the like count for a specific product.
        /// </summary>
        /// <param name="productId">The product ID whose like count is to be incremented.</param>
        /// <returns>True if the like count was incremented successfully; otherwise, false.</returns>
        public virtual bool AddLike(string productId)
        {
            // Loads all products
            var products = GetAllData().ToList();
            // Finds product by ID
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product == null) return false;

            // Increments likes count
            product.Likes++;
            // Saves updated product list
            SaveData(products);
            return true;
        }

        /// <summary>
        /// Resets the like count for a specific product to zero.
        /// </summary>
        /// <param name="productId">The product ID whose like count is to be reset.</param>
        /// <returns> True if the like count was reset successfully ; otherwise False</returns>
        public virtual void ResetLikes(string productId)
        {
            // Loads all products
            var products = GetAllData().ToList();
            // Finds product by ID
            var product = products.FirstOrDefault(p => p.Id == productId);

            // Resets likes count
            product.Likes = 0;
            // Saves updated product list
            SaveData(products);
        }

        /// <summary>
        /// Deletes a product by its ID.
        /// </summary>
        /// <param name="id"> the product Id to delete </param>
        /// <returns> The deleted <see cref="ProductModel"/> if the product was found and deleted; otherwise null </returns>
        public virtual ProductModel DeleteData(string id)
        {
            // Use _testProducts if in test mode
            var productList = _testProducts ?? GetAllData().ToList();
            // Finds product by ID
            var productToDelete = productList.FirstOrDefault(m => m.Id == id);

            // Returns deleted product          
            if (productToDelete == null)
            {
                return null;
            }
            productList.Remove(productToDelete);
            SaveData(productList);
            return productToDelete;
        }

        private IEnumerable<string> GetAvailableSizes()
        {
            return Enum.GetValues(typeof(ProductSize))
                .Cast<ProductSize>().Select(p => p.ToString());
        }

        /// <summary>
        /// Gets all distinct values for categories, sizes, and colors from the product data.
        /// </summary>
        /// <returns>A dictionary of distinct values grouped by property.</returns>
        public Dictionary<string, HashSet<string>> GetAllDataPermutations()
        {
            // Loads all products
            var products = GetAllData();

            return new Dictionary<string, HashSet<string>>
            {
                ["Categories"] = new HashSet<string>(products.Select(p => p.Category.ToString())),
                ["Sizes"] = new HashSet<string>(products.Select(p => p.Size.ToString())),
                ["Colors"] = new HashSet<string>(products.Select(p => p.Color)),
                ["Materials"] = new HashSet<string>(products.SelectMany(p => p.Material)),
                ["Styles"] = new HashSet<string>(products.SelectMany(p => p.Style))
            };


        }

        /// <summary>
        /// Adds a comment to the specified product and updates the data.
        /// </summary>
        /// <param name="productId">The ID of the product to add the comment to.</param>
        /// <param name="comment">The comment to be added.</param>
        public virtual void AddComment(string productId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return;

            var products = GetAllData().ToList(); // Load products from JSON
            var product = products.FirstOrDefault(p => p.Id == productId);

            /*if (product != null)
            {
                product.Comments.Add(comment); // Add new comment
                SaveData(products); // Save updated products list back to JSON
            }*/
            if( product== null)
            {
                return;
            }
            product.Comments.Add(comment); // Add new comment
            SaveData(products); // Save updated products list back to JSON
        }
    }
}