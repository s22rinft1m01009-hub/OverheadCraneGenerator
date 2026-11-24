using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingView
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("width")]
        public double Width { get; set; }

        [JsonPropertyName("height")]
        public double Height { get; set; }

        [JsonPropertyName("scale")]
        public string Scale { get; set; } = "NTS";
    }
}
