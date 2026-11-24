using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingPoint
    {
        [JsonPropertyName("x")]
        public double X { get; set; }

        [JsonPropertyName("y")]
        public double Y { get; set; }

        public DrawingPoint() { }

        public DrawingPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
