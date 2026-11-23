using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.CraneModels;

public sealed class GantryGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design)
    {
        var view = drawing.Views[0];
        var left = view.X + 100;
        var right = view.X + view.Width - 100;
        var ground = view.Y + view.Height - 30;
        var top = view.Y + 70;
        var girderY = top;
        var hoistX = HoistX(left, right, design.HoistPositionMm, design.SpanMm);

        drawing.Elements.Add(new DrawingRectangle { X = left - 12, Y = girderY, Width = right - left + 24, Height = 30, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = girderY + 30, X2 = left - 45, Y2 = ground, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingLine { X1 = right, Y1 = girderY + 30, X2 = right + 45, Y2 = ground, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingRectangle { X = left - 65, Y = ground - 12, Width = 75, Height = 12, Fill = SteelFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = right - 10, Y = ground - 12, Width = 75, Height = 12, Fill = SteelFill, StrokeWidth = 2 });
        AddGround(drawing, left - 80, right + 80, ground);
        drawing.Elements.Add(new DrawingRectangle { X = hoistX - 35, Y = girderY + 30, Width = 70, Height = 28, Fill = HoistFill, StrokeWidth = 2 });
        AddHook(drawing, hoistX, girderY + 58, Math.Max(35, Math.Min(120, design.LiftHeightMm / 100d)));
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design)
    {
        var view = drawing.Views[1];
        var left = view.X + 80;
        var right = view.X + view.Width - 80;
        var centerY = view.Y + 105;
        var footprint = Math.Clamp(design.RunwayWidthMm / 500d, 25, 70);
        var hoistX = HoistX(left, right, design.HoistPositionMm, design.SpanMm);

        drawing.Elements.Add(new DrawingRectangle { X = left, Y = centerY - footprint, Width = right - left, Height = footprint * 2, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = left - 25, Y1 = centerY - footprint - 25, X2 = left - 25, Y2 = centerY + footprint + 25, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingLine { X1 = right + 25, Y1 = centerY - footprint - 25, X2 = right + 25, Y2 = centerY + footprint + 25, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX, CenterY = centerY, Radius = 22, Fill = HoistFill, StrokeWidth = 2 });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design)
    {
        var view = drawing.Views[2];
        var centerX = view.X + view.Width / 2;
        var top = view.Y + 55;
        var legHeight = 100;

        drawing.Elements.Add(new DrawingRectangle { X = centerX - 80, Y = top, Width = 160, Height = 24, Fill = GirderFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = centerX - 65, Y1 = top + 24, X2 = centerX - 85, Y2 = top + legHeight, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingLine { X1 = centerX + 65, Y1 = top + 24, X2 = centerX + 85, Y2 = top + legHeight, StrokeWidth = 4 });
        drawing.Elements.Add(new DrawingRectangle { X = centerX - 105, Y = top + legHeight, Width = 45, Height = 12, Fill = SteelFill, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingRectangle { X = centerX + 60, Y = top + legHeight, Width = 45, Height = 12, Fill = SteelFill, StrokeWidth = 2 });
    }
}
