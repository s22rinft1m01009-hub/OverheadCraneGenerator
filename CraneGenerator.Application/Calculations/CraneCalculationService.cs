using CraneGenerator.Application.DTOs;

namespace CraneGenerator.Application.Calculations;

public sealed class CraneCalculationService : ICraneCalculationService
{
    public CraneCalculationResult Calculate(DesignResponse design)
    {
        ArgumentNullException.ThrowIfNull(design);

        var hoistPositionRatio = design.SpanMm <= 0
            ? 0
            : Math.Clamp(design.HoistPositionMm / design.SpanMm, 0, 1);
        var spanDrawingWidth = 640d;
        var buildingHeightDrawing = Math.Clamp(design.BuildingHeightMm / 100000d * 170d, 90d, 170d);
        var liftRopeLength = Math.Clamp(design.LiftHeightMm / 100d, 35d, 120d);
        var runwayDrawingDepth = Math.Clamp(design.RunwayWidthMm / 1000d, 25d, 70d);
        var scaleDenominator = design.SpanMm <= 0 ? 1 : Math.Max(1, Math.Ceiling(design.SpanMm / spanDrawingWidth));

        return new CraneCalculationResult
        {
            HoistPositionRatio = hoistPositionRatio,
            SpanDrawingWidth = spanDrawingWidth,
            HoistDrawingX = 100 + hoistPositionRatio * spanDrawingWidth,
            BuildingHeightDrawing = buildingHeightDrawing,
            LiftRopeLength = liftRopeLength,
            RunwayDrawingDepth = runwayDrawingDepth,
            DrawingScale = $"1:{scaleDenominator:0}"
        };
    }
}
