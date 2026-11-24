using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingSheetMetadata
    {
        [JsonPropertyName("sheetWidth")]
        public double SheetWidth { get; set; } = 1200;

        [JsonPropertyName("sheetHeight")]
        public double SheetHeight { get; set; } = 800;

        [JsonPropertyName("outerMargin")]
        public double OuterMargin { get; set; } = 15;

        [JsonPropertyName("innerMargin")]
        public double InnerMargin { get; set; } = 30;

        [JsonPropertyName("gridColumns")]
        public string GridColumns { get; set; } = "A,B,C,D,E,F,G,H";

        [JsonPropertyName("gridRows")]
        public string GridRows { get; set; } = "1,2,3,4,5,6";

        [JsonPropertyName("warningNotice")]
        public string WarningNotice { get; set; } = "PRELIMINARY DRAWING - CONCEPTUAL / OFFER ONLY - NOT FOR FABRICATION";

        [JsonPropertyName("projectTitle")]
        public string ProjectTitle { get; set; } = "PARAMETRIC OVERHEAD CRANE";

        [JsonPropertyName("units")]
        public string Units { get; set; } = "MILLIMETERS (mm)";
    }
}
