using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingDimension
    {
        [JsonPropertyName("x1")]
        public double X1 { get; set; }

        [JsonPropertyName("y1")]
        public double Y1 { get; set; }

        [JsonPropertyName("x2")]
        public double X2 { get; set; }

        [JsonPropertyName("y2")]
        public double Y2 { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("offset")]
        public double Offset { get; set; } = 20;

        [JsonPropertyName("isReference")]
        public bool IsReference { get; set; } = false;

        [JsonPropertyName("orientation")]
        public string Orientation { get; set; } = "horizontal";
    }
}
