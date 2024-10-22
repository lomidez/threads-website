using System.Collections.Generic;
namespace ContosoCrafts.WebSite.Models
{
    public class TagModel
    {
        public int TagID { get; set; } // unique ID for each tag
        public string TagName { get; set; } // Name of the tag (e.g., "shoes", " t-shirt" , "jacket")
        public string TagCategory { get; set; } //Category: Type, size, Color, Material, Style etc.
        public List<string> TagValues { get; set; } //i.e size has small, medium, large
    }
}
