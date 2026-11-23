namespace CraneGenerator.Application.Calculations;

public sealed record CraneCalculationResult
{
    public double HoistPositionRatio { get; init; }
    public double SpanDrawingWidth { get; init; }
    public double HoistDrawingX { get; init; }
    public double BuildingHeightDrawing { get; init; }
    public double LiftRopeLength { get; init; }
    public double RunwayDrawingDepth { get; init; }
    public string DrawingScale { get; init; } = "NTS";
}
