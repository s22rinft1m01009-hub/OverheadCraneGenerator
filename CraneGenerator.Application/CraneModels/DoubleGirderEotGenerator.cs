using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.CraneModels;

public sealed class DoubleGirderEotGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var (left, right, ground, top) = ElevationBounds(view, calculation);
        var girderLeft = left + 25;
        var girderWidth = right - left - 50;
        var upperY = top + 35;
        var lowerY = upperY + 45;
        var hoistX = HoistX(girderLeft, girderLeft + girderWidth, calculation.HoistPositionRatio);

        drawing.Elements.Add(new DrawingRectangle { X = left, Y = top, Width = 25, Height = ground - top, Fill = SteelFill });
        drawing.Elements.Add(new DrawingRectangle { X = right - 25, Y = top, Width = 25, Height = ground - top, Fill = SteelFill });
        AddGround(drawing, left, right, ground);
        drawing.Elements.Add(new DrawingRectangle { X = girderLeft, Y = upperY, Width = girderWidth, Height = 24, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = girderLeft, Y = lowerY, Width = girderWidth, Height = 24, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = hoistX - 40, Y = upperY - 5, Width = 80, Height = lowerY - upperY + 34, Fill = HoistFill, StrokeWidth = 2 });
        AddHook(drawing, hoistX, lowerY + 24, calculation.LiftRopeLength);
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 50;
        var right = view.X + view.Width - 50;
        var centerY = view.Y + 105;
        var separation = Math.Clamp(design.RunwayWidthMm / 400d, 25, 55);
        var hoistX = HoistX(left, right, calculation.HoistPositionRatio);

        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY - separation, X2 = right, Y2 = centerY - separation, StrokeWidth = 5 });
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY + separation, X2 = right, Y2 = centerY + separation, StrokeWidth = 5 });
        drawing.Elements.Add(new DrawingRectangle { X = hoistX - 38, Y = centerY - separation - 20, Width = 76, Height = separation * 2 + 40, Fill = HoistFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY - 75, X2 = right, Y2 = centerY - 75, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY + 75, X2 = right, Y2 = centerY + 75, StrokeWidth = 2 });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var centerX = view.X + view.Width / 2;
        var top = view.Y + 55;
        var separation = Math.Clamp(calculation.RunwayDrawingDepth / 2d, 30, 65);

        drawing.Elements.Add(new DrawingRectangle { X = centerX - separation - 50, Y = top, Width = 100, Height = 22, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = centerX - separation - 50, Y = top + 50, Width = 100, Height = 22, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = centerX - 35, Y = top + 20, Width = 70, Height = 45, Fill = HoistFill, StrokeWidth = 2 });
    }
}
