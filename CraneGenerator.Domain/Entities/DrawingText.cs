using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingText : DrawingElement
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("fontSize")]
        public double FontSize { get; set; } = 10;

        [JsonPropertyName("fill")]
        public string Fill { get; set; } = "#000000";

        [JsonPropertyName("textAnchor")]
        public string TextAnchor { get; set; } = "start";

        [JsonPropertyName("fontWeight")]
        public string FontWeight { get; set; } = "normal";
    }
}
