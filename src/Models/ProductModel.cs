
//Below is the product model without Rating functionality 
//using System.Text.Json;
//using System.Text.Json.Serialization;
//using System.ComponentModel.DataAnnotations;
//using System.Collections.Generic;
//using ContosoCrafts.WebSite.Models;


//namespace ContosoCrafts.WebSite.Models
//{
//    public class ProductModel
//    {
//        public string Id { get; set; }
//        public string Maker { get; set; }

//        [JsonPropertyName("img")]
//        public string Image { get; set; }

//        public string Url { get; set; }

//        [StringLength(maximumLength: 33, MinimumLength = 1, ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
//        public string Title { get; set; }

//        public string Description { get; set; }

//        // Delete Ranking Functionality
//        //public int[] Ratings { get; set; }

//        public ProductTypeEnum ProductType { get; set; } = ProductTypeEnum.Undefined;

//        public string Quantity { get; set; }

//        [Range(-1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
//        public int Price { get; set; }

//        // Store the Comments entered by the users on this product
//        public List<CommentModel> CommentList { get; set; } = new List<CommentModel>();

//        // Store the tags associated with this product (fixed the typo)
//        public List<TagModel> Tags { get; set; } = new List<TagModel>();

//        // Store all tags for Category, Size, Color, Material, and Style
//        public string Category { get; set; }
//        public string Size { get; set; }
//        public string Color { get; set; }
//        public List<string> Material { get; set; } = new List<string>();
//        public List<string> Style { get; set; } = new List<string>();

//        // ToString method to serialize the ProductModel (only one instance now)
//        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);
//    }
//}

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
        public string Image { get; set; }

        // URL link to the product details page
        public string Url { get; set; }

        // Title of the product with validation constraints on length
        [StringLength(maximumLength: 64, MinimumLength = 1, ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
        public string Title { get; set; }

        // Description providing details about the product
        public string Description { get; set; }

        // Array of ratings, aligns with JSON data format (nullable int array if data may be missing)
        public int[] Ratings { get; set; } = new int[] { };

        // Number of Likes for the product
        public int Likes { get; set; } = 0;

        // Enum to specify the type of product, defaults to Undefined if not specified
        public ProductTypeEnum ProductType { get; set; } = ProductTypeEnum.Undefined;

        // Quantity available, nullable to handle missing values in JSON data
        public int? Quantity { get; set; }

        // Price of the product with validation to ensure non-negative values
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be non-negative.")]
        public int Price { get; set; }

        // List of user comments on the product
        public List<CommentModel> CommentList { get; set; } = new List<CommentModel>();

        // List of tags associated with the product, such as attributes like size or color
        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        // Category of the product, e.g., electronics, clothing
        public string Category { get; set; }

        // Size attribute to specify product size
        public string Size { get; set; }

        // Color attribute to specify product color
        public string Color { get; set; }

        // Material list for detailing what materials the product is made from
        public List<string> Material { get; set; } = new List<string>();

        // Style list to specify design styles associated with the product
        public List<string> Style { get; set; } = new List<string>();

        // Weight of the product, stored as a decimal for precision
        public decimal Weight { get; set; }

        // Dimensions of the product, with length, width, and height specified in a separate class
        public Dimensions Dimentions { get; set; } = new Dimensions();

        /// <summary>
        /// Serializes the ProductModel to a JSON string representation.
        /// </summary>
        /// <returns>JSON string representation of the product.</returns>
        public override string ToString() => JsonSerializer.Serialize(this);
    }

    /// <summary>
    /// Represents the dimensions of a product with length, width, and height properties.
    /// </summary>
    public class Dimensions
    {
        // Length of the product
        public decimal Length { get; set; }

        // Width of the product
        public decimal Width { get; set; }

        // Height of the product
        public decimal Height { get; set; }
    }
}



