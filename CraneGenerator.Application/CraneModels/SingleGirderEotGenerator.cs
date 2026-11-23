using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.CraneModels;

public sealed class SingleGirderEotGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var (left, right, ground, top) = ElevationBounds(view, calculation);
        var girderY = top + 45;
        var girderLeft = left + 25;
        var girderWidth = right - left - 50;
        var hoistX = HoistX(girderLeft, girderLeft + girderWidth, calculation.HoistPositionRatio);

        drawing.Elements.Add(new DrawingRectangle { X = left, Y = top, Width = 25, Height = ground - top, Fill = SteelFill });
        drawing.Elements.Add(new DrawingRectangle { X = right - 25, Y = top, Width = 25, Height = ground - top, Fill = SteelFill });
        AddGround(drawing, left, right, ground);
        drawing.Elements.Add(new DrawingRectangle { X = girderLeft, Y = girderY, Width = girderWidth, Height = 30, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = hoistX - 30, Y = girderY - 15, Width = 60, Height = 30, Fill = HoistFill, StrokeWidth = 2 });
        AddHook(drawing, hoistX, girderY + 15, calculation.LiftRopeLength);
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 50;
        var right = view.X + view.Width - 50;
        var centerY = view.Y + 105;
        var runwayDepth = calculation.RunwayDrawingDepth;
        var hoistX = HoistX(left, right, calculation.HoistPositionRatio);

        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY - runwayDepth, X2 = right, Y2 = centerY - runwayDepth, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = centerY + runwayDepth, X2 = right, Y2 = centerY + runwayDepth, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingRectangle { X = left, Y = centerY - 15, Width = right - left, Height = 30, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = hoistX - 25, Y = centerY - 28, Width = 50, Height = 56, Fill = HoistFill, StrokeWidth = 2 });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var centerX = view.X + view.Width / 2;
        var top = view.Y + 55;
        var depth = Math.Clamp(calculation.RunwayDrawingDepth, 35, 100);

        drawing.Elements.Add(new DrawingRectangle { X = centerX - depth / 2, Y = top, Width = depth, Height = 28, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = centerX - 25, Y = top - 20, Width = 50, Height = 25, Fill = HoistFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = centerX, Y1 = top + 28, X2 = centerX, Y2 = view.Y + view.Height - 30, StrokeWidth = 2 });
    }
}
