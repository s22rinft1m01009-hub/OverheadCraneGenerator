using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingPolyline : DrawingElement
    {
        [JsonPropertyName("points")]
        public List<DrawingPoint> Points { get; set; } = [];

        [JsonPropertyName("isClosed")]
        public bool IsClosed { get; set; } = false;

        [JsonPropertyName("fill")]
        public string Fill { get; set; } = "none";
    }
}
