using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingRectangle : DrawingElement
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("width")]
        public double Width { get; set; }

        [JsonPropertyName("height")]
        public double Height { get; set; }

        [JsonPropertyName("fill")]
        public string Fill { get; set; } = "none";
    }
}
