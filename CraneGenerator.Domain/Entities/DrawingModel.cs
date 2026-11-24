using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingModel
    {
        [JsonPropertyName("drawingNumber")]
        public string DrawingNumber { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("scale")]
        public string Scale { get; set; } = "NTS";

        [JsonPropertyName("revision")]
        public string Revision { get; set; } = "A";

        [JsonPropertyName("views")]
        public List<DrawingView> Views { get; set; } = [];

        [JsonPropertyName("elements")]
        public List<DrawingElement> Elements { get; set; } = [];

        [JsonPropertyName("dimensions")]
        public List<DrawingDimension> Dimensions { get; set; } = [];

        [JsonPropertyName("details")]
        public List<DrawingDetail> Details { get; set; } = [];

        [JsonPropertyName("notes")]
        public List<DrawingNote> Notes { get; set; } = [];

        [JsonPropertyName("revisions")]
        public List<DrawingRevision> Revisions { get; set; } = [];

        [JsonPropertyName("technicalData")]
        public DrawingTechnicalData TechnicalData { get; set; } = new();

        [JsonPropertyName("sheetMetadata")]
        public DrawingSheetMetadata SheetMetadata { get; set; } = new();
    }
}
