using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(DrawingLine), "line")]
    [JsonDerivedType(typeof(DrawingRectangle), "rectangle")]
    [JsonDerivedType(typeof(DrawingCircle), "circle")]
    [JsonDerivedType(typeof(DrawingPolyline), "polyline")]
    [JsonDerivedType(typeof(DrawingArc), "arc")]
    [JsonDerivedType(typeof(DrawingText), "text")]
    [JsonDerivedType(typeof(DrawingCenterline), "centerline")]
    public class DrawingElement
    {
        [JsonPropertyName("stroke")]
        public string Stroke { get; set; } = "#000000";

        [JsonPropertyName("strokeWidth")]
        public double StrokeWidth { get; set; } = 1;
    }
}
