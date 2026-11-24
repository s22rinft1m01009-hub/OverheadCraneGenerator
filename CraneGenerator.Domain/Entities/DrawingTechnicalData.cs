using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CraneGenerator.Domain.Entities
{
    public enum SpecificationSource
    {
        UserInput,
        Derived,
        Estimate,
        PrototypeRule,
        Reference
    }

    public class TechnicalSpecificationItem
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;

        [JsonPropertyName("value")]
        public string Value { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public SpecificationSource Source { get; set; } = SpecificationSource.Estimate;

        [JsonPropertyName("sourceTag")]
        public string SourceTag => Source switch
        {
            SpecificationSource.UserInput => "[USER INPUT]",
            SpecificationSource.Derived => "[DERIVED]",
            SpecificationSource.Estimate => "[ESTIMATE]",
            SpecificationSource.PrototypeRule => "[PROTOTYPE]",
            _ => "[REFERENCE]"
        };

        public TechnicalSpecificationItem() { }

        public TechnicalSpecificationItem(string label, string value, SpecificationSource source)
        {
            Label = label;
            Value = value;
            Source = source;
        }
    }

    public class DrawingTechnicalData 
    {
        [JsonPropertyName("designName")]
        public string DesignName { get; set; } = string.Empty;

        [JsonPropertyName("craneType")]
        public string CraneType { get; set; } = string.Empty;

        [JsonPropertyName("drawingNumber")]
        public string DrawingNumber { get; set; } = string.Empty;

        [JsonPropertyName("revision")]
        public string Revision { get; set; } = "A";

        [JsonPropertyName("note")]
        public string Note { get; set; } = "CONCEPTUAL DRAWING - NOT FOR FABRICATION";

        [JsonPropertyName("capacityTonnes")]
        public double CapacityTonnes { get; set; }

        [JsonPropertyName("spanMm")]
        public double SpanMm { get; set; }

        [JsonPropertyName("liftHeightMm")]
        public double LiftHeightMm { get; set; }

        [JsonPropertyName("runwayWidthMm")]
        public double RunwayWidthMm { get; set; }

        [JsonPropertyName("hoistPositionMm")]
        public double HoistPositionMm { get; set; }

        [JsonPropertyName("buildingHeightMm")]
        public double BuildingHeightMm { get; set; }

        // Speeds
        [JsonPropertyName("hoistingSpeed")]
        public string HoistingSpeed { get; set; } = "6/1 m/min 2-SPEED";

        [JsonPropertyName("traversingSpeed")]
        public string TraversingSpeed { get; set; } = "0-20 m/min STEPLESS";

        [JsonPropertyName("travellingSpeed")]
        public string TravellingSpeed { get; set; } = "0-32 m/min STEPLESS";

        // Weights
        [JsonPropertyName("trolleyWeight")]
        public string TrolleyWeight { get; set; } = "0.68 t";

        [JsonPropertyName("craneWeight")]
        public string CraneWeight { get; set; } = "8.9 t";

        // Power Supply
        [JsonPropertyName("powerSupply")]
        public string PowerSupply { get; set; } = "380V / 48V / 50Hz";

        [JsonPropertyName("totalSupplyPower")]
        public string TotalSupplyPower { get; set; } = "14.3 kW";

        // Groups / Classification
        [JsonPropertyName("craneGroup")]
        public string CraneGroup { get; set; } = "FEM A5 (ISO M5)";

        [JsonPropertyName("hoistGroup")]
        public string HoistGroup { get; set; } = "M5 (FEM 2m)";

        [JsonPropertyName("bridgeTravelGroup")]
        public string BridgeTravelGroup { get; set; } = "M5 (FEM 2m)";

        // Standards & Paint
        [JsonPropertyName("tolerance")]
        public string Tolerance { get; set; } = "ACC. TO ISO/FEM STANDARD";

        [JsonPropertyName("bridgePaint")]
        public string BridgePaint { get; set; } = "E120 / RAL1028 (MELON YELLOW)";

        [JsonPropertyName("specifications")]
        public List<TechnicalSpecificationItem> Specifications { get; set; } = [];
    }
}
