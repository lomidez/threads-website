using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ContosoCrafts.WebSite.Models
{
    /// <summary>
    /// Represents a the XYZ dimensions of a model
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



