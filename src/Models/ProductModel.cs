using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ContosoCrafts.WebSite.Models
{
    /// <summary>
    /// Represents a product with various attributes such as title, price, description, ratings, and more.
    /// </summary>
    public class ProductModel
    {
        // Unique identifier for each product
        public string Id { get; set; }

        // Manufacturer of the product
        public string Maker { get; set; }

        // Product image URL, maps to the JSON property "img"
        [JsonPropertyName("img")]
        [Required(ErrorMessage = "Image is required!")]
        public string Image { get; set; }

        // URL link to the product details page
        public string Url { get; set; }

        // Title of the product with validation constraints on length
        [Required(ErrorMessage = "Title is required!")]
        [StringLength(maximumLength: 64, MinimumLength = 1, ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
        public string Title { get; set; }

        // Description providing details about the product
        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }

        // Array of ratings, aligns with JSON data format (nullable int array if data may be missing)
        public int[] Ratings { get; set; } = new int[] { };

        // Number of Likes for the product
        public int Likes { get; set; } = 0;

        // New Comments property
        public List<string> Comments { get; set; } = new List<string>();

        // Quantity available, nullable to handle missing values in JSON data
        public int? Quantity { get; set; }

        // Price of the product with validation to ensure non-negative values
        [Required(ErrorMessage = "Price is required!")]
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be non-negative.")]
        public int Price { get; set; }

        // List of user comments on the product
        public List<CommentModel> CommentList { get; set; } = new List<CommentModel>();

        // Category of the product, e.g., electronics, clothing
        [Required(ErrorMessage = "Category is required!")]
        public string Category { get; set; }

        // Size attribute to specify product size
        [Required(ErrorMessage = "Size is required!")]
        [EnumDataType(typeof(ProductSize), ErrorMessage = "Invalid size value")]
        public ProductSize Size { get; set; }


        // Color attribute to specify product color
        [Required(ErrorMessage = "Color is required!")]
        [EnumDataType(typeof(ProductColor), ErrorMessage = "Invalid color value")]
        public string Color { get; set; }

        // Material list to specify the material associated with the product
        public List<string> Material { get; set; } = new List<string>();



        // Style list to specify design styles associated with the product
        [Required(ErrorMessage = "At least one style is required!")]
        public List<string> Style { get; set; } = new List<string>();

        // Weight of the product, stored as a decimal for precision
        public decimal Weight { get; set; }

        // Dimensions of the product, with length, width, and height specified in a separate class
        public Dimensions Dimentions { get; set; } = new Dimensions();

        // Numeric Field for valid data inputs
        public int NumericField { get; set; }

        /// <summary>
        /// Serializes the ProductModel to a JSON string representation.
        /// </summary>
        /// <returns>JSON string representation of the product.</returns>
        public override string ToString() => JsonSerializer.Serialize(this);
    }
}



