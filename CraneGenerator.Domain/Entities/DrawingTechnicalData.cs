namespace CraneGenerator.Domain.Entities
{
    public class DrawingTechnicalData 
    {
        public string DesignName { get; set; } = string.Empty;

        public string CraneType { get; set; } = string.Empty;

        public string DrawingNumber { get; set; } = string.Empty;

        public string Revision { get; set; } = "A";

        public string Note { get; set; } = "CONCEPTUAL DRAWING - NOT FOR FABRICATION";

        public double CapacityTonnes { get; set; }

        public double SpanMm { get; set; }

        public double LiftHeightMm { get; set; }

        public double RunwayWidthMm { get; set; }

        public double HoistPositionMm { get; set; }

        public double BuildingHeightMm { get; set; }
    }
}
