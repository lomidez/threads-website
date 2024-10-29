using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ContosoCrafts.WebSite.Models;


namespace ContosoCrafts.WebSite.Models
{
    public class ProductModel
    {
        public string Id { get; set; }
        public string Maker { get; set; }

        [JsonPropertyName("img")]
        public string Image { get; set; }

        public string Url { get; set; }

        [StringLength(maximumLength: 33, MinimumLength = 1, ErrorMessage = "The Title should have a length of more than {2} and less than {1}")]
        public string Title { get; set; }

        public string Description { get; set; }

        // Delete Ranking Functionality
        //public int[] Ratings { get; set; }

        public ProductTypeEnum ProductType { get; set; } = ProductTypeEnum.Undefined;

        public string Quantity { get; set; }

        [Range(-1, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Price { get; set; }

        // Store the Comments entered by the users on this product
        public List<CommentModel> CommentList { get; set; } = new List<CommentModel>();

        // Store the tags associated with this product (fixed the typo)
        public List<TagModel> Tags { get; set; } = new List<TagModel>();

        // Store all tags for Category, Size, Color, Material, and Style
        public string Category { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public List<string> Material { get; set; } = new List<string>();
        public List<string> Style { get; set; } = new List<string>();

        // ToString method to serialize the ProductModel (only one instance now)
        public override string ToString() => JsonSerializer.Serialize<ProductModel>(this);
    }
}
