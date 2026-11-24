using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingRevision
    {
        [JsonPropertyName("rev")]
        public string Rev { get; set; } = "A";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "INITIAL CONCEPTUAL DRAWING";

        [JsonPropertyName("date")]
        public string Date { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

        [JsonPropertyName("designed")]
        public string Designed { get; set; } = "AGY";

        [JsonPropertyName("checked")]
        public string Checked { get; set; } = "ENG";
    }
}
