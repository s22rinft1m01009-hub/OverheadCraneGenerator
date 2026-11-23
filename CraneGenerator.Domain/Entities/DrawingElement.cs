using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(DrawingLine), "line")]
    [JsonDerivedType(typeof(DrawingRectangle), "rectangle")]
    [JsonDerivedType(typeof(DrawingCircle), "circle")]
    public class DrawingElement
    {
        public string Stroke { get; set; } = "#000000";

        public double StrokeWidth { get; set; } = 1;
    }
}
