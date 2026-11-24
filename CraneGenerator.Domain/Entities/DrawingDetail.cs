using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingDetail
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("scale")]
        public string Scale { get; set; } = "NTS";

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("width")]
        public double Width { get; set; }

        [JsonPropertyName("height")]
        public double Height { get; set; }

        [JsonPropertyName("elements")]
        public List<DrawingElement> Elements { get; set; } = [];

        [JsonPropertyName("dimensions")]
        public List<DrawingDimension> Dimensions { get; set; } = [];
    }
}
