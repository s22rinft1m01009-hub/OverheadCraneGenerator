using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingArc : DrawingElement
    {
        [JsonPropertyName("centerX")]
        public double CenterX { get; set; }

        [JsonPropertyName("centerY")]
        public double CenterY { get; set; }

        [JsonPropertyName("radius")]
        public double Radius { get; set; }

        [JsonPropertyName("startAngleDegrees")]
        public double StartAngleDegrees { get; set; }

        [JsonPropertyName("endAngleDegrees")]
        public double EndAngleDegrees { get; set; }

        [JsonPropertyName("fill")]
        public string Fill { get; set; } = "none";
    }
}
