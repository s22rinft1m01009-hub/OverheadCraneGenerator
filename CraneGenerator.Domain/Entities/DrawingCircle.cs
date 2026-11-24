using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingCircle : DrawingElement
    {
        [JsonPropertyName("centerX")]
        public double CenterX { get; set; }

        [JsonPropertyName("centerY")]
        public double CenterY { get; set; }

        [JsonPropertyName("radius")]
        public double Radius { get; set; }

        [JsonPropertyName("fill")]
        public string Fill { get; set; } = "none";
    }
}
