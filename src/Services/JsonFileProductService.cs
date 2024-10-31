using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ContosoCrafts.WebSite.Models;
using Microsoft.AspNetCore.Hosting;

namespace ContosoCrafts.WebSite.Services
{
    public class JsonFileProductService
    {
        private List<ProductModel> _testProducts; // Field for holding mock data during tests

        // Constructor for regular use (without mock data)
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        // Optional constructor for injecting mock data for testing
        public JsonFileProductService(IWebHostEnvironment webHostEnvironment, List<ProductModel> testProducts)
        {
            WebHostEnvironment = webHostEnvironment;
            _testProducts = testProducts; // Assign test data
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        private string JsonFileName
        {
            get
            {
                // Find the path relative to the project root for consistent access during tests
                return Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "products.json");
            }
        }

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


        // Update SaveData to public to allow testing modifications
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

        public ProductModel CreateData(ProductModel data)
        {
            var dataSet = GetAllData().Append(data);

            SaveData(dataSet);

            return data;
        }

        public virtual ProductModel DeleteData(string id)
        {
            var dataSet = GetAllData().Where(m => m.Id != id);
            var data = GetAllData().FirstOrDefault(m => m.Id.Equals(id));

            SaveData(dataSet);

            return data;
        }
    }
}


