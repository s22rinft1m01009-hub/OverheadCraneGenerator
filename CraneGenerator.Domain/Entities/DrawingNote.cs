using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingNote
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = "GENERAL";
    }
}
