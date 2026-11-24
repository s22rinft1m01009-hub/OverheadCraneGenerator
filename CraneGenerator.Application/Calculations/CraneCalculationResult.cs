namespace CraneGenerator.Application.Calculations;

public sealed record CraneCalculationResult
{
    // Input parameters echo
    public double SpanMm { get; init; }
    public double LiftHeightMm { get; init; }
    public double RunwayWidthMm { get; init; }
    public double BuildingHeightMm { get; init; }
    public double HoistPositionMm { get; init; }
    public double CapacityTonnes { get; init; }

    // Proportional & drawing scale values
    public double HoistPositionRatio { get; init; }
    public double DrawingScaleDenominator { get; init; }
    public string DrawingScale { get; init; } = "NTS";
    public double SpanDrawingWidth { get; init; } = 580;
    public double DrawingSpan { get; init; } = 580;
    public double DrawingHeight { get; init; } = 200;
    public double BuildingHeightDrawing { get; init; }
    public double CraneElevation { get; init; }
    public double LiftRopeLength { get; init; }
    public double RunwayDrawingDepth { get; init; }
    public double HoistDrawingX { get; init; }

    // Clearances (derived / reference estimates)
    public double SideClearanceLeftMm { get; init; } = 235;
    public double SideClearanceRightMm { get; init; } = 235;
    public double OverheadClearanceMm { get; init; } = 200;
    public double HookApproachLeftMm { get; init; } = 920;
    public double HookApproachRightMm { get; init; } = 1150;

    // Technical specifications (estimates / prototype rules)
    public string HoistingSpeed { get; init; } = "6/1 m/min 2-SPEED";
    public string TraversingSpeed { get; init; } = "0-20 m/min STEPLESS";
    public string TravellingSpeed { get; init; } = "0-32 m/min STEPLESS";
    public double EstimatedTrolleyWeightTonnes { get; init; }
    public double EstimatedCraneWeightTonnes { get; init; }
    public string PowerSupply { get; init; } = "380V / 48V / 50Hz";
    public double EstimatedTotalPowerKw { get; init; }
    public string CraneGroup { get; init; } = "FEM A5 (ISO M5)";
    public string HoistGroup { get; init; } = "M5 (FEM 2m)";
    public string BridgeTravelGroup { get; init; } = "M5 (FEM 2m)";
    public string ToleranceStandard { get; init; } = "ACC. TO ISO/FEM STANDARD";
    public string BridgePaint { get; init; } = "E120 / RAL1028 (MELON YELLOW)";
}
