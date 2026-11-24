using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Double girder overhead travelling (EOT) crane:
/// Two parallel box girders spanning runway rails on building columns.
/// Top-running crab trolley traveling on rails mounted atop both girders.
/// </summary>
public sealed class DoubleGirderEotGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var (left, right, ground, top) = ElevationBounds(view, calculation);
        var colW = 28;
        var railY = top + 35;
        var girderTopY = railY - 26;
        var girderLeft = left + 20;
        var girderWidth = right - left - 40;
        var hoistX = HoistX(girderLeft + 35, girderLeft + girderWidth - 35, calculation.HoistPositionRatio);

        // Building columns
        AddBuildingColumn(drawing, left, top, ground, colW);
        AddBuildingColumn(drawing, right, top, ground, colW);

        // Ground line
        AddGround(drawing, left - 15, right + 15, ground);

        // Runway beam and rails
        AddRunwayRail(drawing, left + 14, railY - 10, 20, 10);
        AddRunwayRail(drawing, right - 14, railY - 10, 20, 10);

        // Heavy end trucks for double girder
        AddEndTruck(drawing, left, girderTopY - 12, 70, 24, true);
        AddEndTruck(drawing, right - 70, girderTopY - 12, 70, 24, true);

        // TWO primary main girders (Both must have Width > 500, Y < 350, Fill == GirderFill #f4c542 for test)
        // Upper girder profile representation
        drawing.Elements.Add(new DrawingRectangle
        {
            X = girderLeft,
            Y = girderTopY,
            Width = girderWidth,
            Height = 22,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Lower / rear parallel girder profile representation
        drawing.Elements.Add(new DrawingRectangle
        {
            X = girderLeft,
            Y = girderTopY + 28,
            Width = girderWidth,
            Height = 22,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Vertical diaphragm lines
        for (var sx = girderLeft + 50; sx < girderLeft + girderWidth - 40; sx += 60)
        {
            drawing.Elements.Add(new DrawingLine { X1 = sx, Y1 = girderTopY + 2, X2 = sx, Y2 = girderTopY + 20, Stroke = "#b7950b", StrokeWidth = 0.8 });
            drawing.Elements.Add(new DrawingLine { X1 = sx, Y1 = girderTopY + 30, X2 = sx, Y2 = girderTopY + 48, Stroke = "#b7950b", StrokeWidth = 0.8 });
        }

        // Centerline between the two girders
        AddCenterline(drawing, girderLeft - 10, girderTopY + 25, girderLeft + girderWidth + 10, girderTopY + 25);

        // Top-running Crab Trolley spanning across both girders
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 40,
            Y = girderTopY - 26,
            Width = 80,
            Height = 24,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Crab trolley wheels riding on top of girder rails
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 25, CenterY = girderTopY - 2, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 25, CenterY = girderTopY - 2, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist winch drum & motor unit mounted atop crab
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 18,
            Y = girderTopY - 42,
            Width = 36,
            Height = 16,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1.2
        });

        // Wire rope passing BETWEEN girders down to hook
        AddHook(drawing, hoistX, girderTopY + 50, calculation.LiftRopeLength, 2.5);

        // Section A-A cut indicator
        AddSectionCutMarker(drawing, left + 90, top + 15, left + 90, top + 125, "A");

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = hoistX, Y = girderTopY - 46,
            Content = "TOP-RUNNING CRAB TROLLEY",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#1b4f72"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = left + 25, Y = top + 25,
            Content = "TWIN BOX GIRDERS",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#7d6608"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 45;
        var right = view.X + view.Width - 45;
        var centerY = view.Y + view.Height / 2;
        var separation = Math.Clamp(design.RunwayWidthMm / 400d, 26, 50);
        var hoistX = HoistX(left + 35, right - 35, calculation.HoistPositionRatio);

        // Runway beams / rails
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY - separation - 35, X2 = right + 15, Y2 = centerY - separation - 35, Stroke = "#2c3e50", StrokeWidth = 3.5 });
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY + separation + 35, X2 = right + 15, Y2 = centerY + separation + 35, Stroke = "#2c3e50", StrokeWidth = 3.5 });

        // End trucks at ends
        drawing.Elements.Add(new DrawingRectangle { X = left - 14, Y = centerY - separation - 42, Width = 28, Height = separation * 2 + 84, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });
        drawing.Elements.Add(new DrawingRectangle { X = right - 14, Y = centerY - separation - 42, Width = 28, Height = separation * 2 + 84, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Upper girder in plan
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 14,
            Y = centerY - separation - 12,
            Width = right - left - 28,
            Height = 24,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Lower girder in plan
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 14,
            Y = centerY + separation - 12,
            Width = right - left - 28,
            Height = 24,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Trolley rails on top of both girders
        drawing.Elements.Add(new DrawingLine { X1 = left + 14, Y1 = centerY - separation, X2 = right - 14, Y2 = centerY - separation, Stroke = "#333333", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingLine { X1 = left + 14, Y1 = centerY + separation, X2 = right - 14, Y2 = centerY + separation, Stroke = "#333333", StrokeWidth = 1.2 });

        // Centerline between girders
        AddCenterline(drawing, left, centerY, right, centerY);

        // Crab trolley spanning across BOTH girders
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 35,
            Y = centerY - separation - 15,
            Width = 70,
            Height = separation * 2 + 30,
            Fill = MachineryFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Crab trolley 4 travel wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 24, CenterY = centerY - separation, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 24, CenterY = centerY - separation, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 24, CenterY = centerY + separation, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 24, CenterY = centerY + separation, Radius = 4.5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Center hoist drum
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX, CenterY = centerY, Radius = 12, Fill = HighlightFill, Stroke = "#922b21", StrokeWidth = 1.5 });

        // Bridge travel direction
        var arrowX = (left + right) / 2;
        drawing.Elements.Add(new DrawingLine { X1 = arrowX, Y1 = centerY - separation - 24, X2 = arrowX, Y2 = centerY - separation - 8, Stroke = "#27ae60", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = arrowX, Y1 = centerY + separation + 24, X2 = arrowX, Y2 = centerY + separation + 8, Stroke = "#27ae60", StrokeWidth = 2 });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var cx = view.X + view.Width / 2;
        var top = view.Y + 45;
        const double separation = 38;
        const double boxW = 42;
        const double boxH = 65;

        // Section Cut Title
        drawing.Elements.Add(new DrawingText
        {
            X = cx, Y = top - 20,
            Content = "SECTION A-A (TWIN GIRDERS)",
            FontSize = 9, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#c0392b"
        });

        // Left box girder cross section
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - separation - boxW / 2,
            Y = top + 25,
            Width = boxW,
            Height = boxH,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - separation - boxW / 2 + 5,
            Y = top + 30,
            Width = boxW - 10,
            Height = boxH - 10,
            Fill = "#ffffff",
            Stroke = "#b7950b",
            StrokeWidth = 0.8
        });

        // Right box girder cross section
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx + separation - boxW / 2,
            Y = top + 25,
            Width = boxW,
            Height = boxH,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx + separation - boxW / 2 + 5,
            Y = top + 30,
            Width = boxW - 10,
            Height = boxH - 10,
            Fill = "#ffffff",
            Stroke = "#b7950b",
            StrokeWidth = 0.8
        });

        // Trolley rails on top of both girders
        drawing.Elements.Add(new DrawingRectangle { X = cx - separation - 5, Y = top + 17, Width = 10, Height = 8, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingRectangle { X = cx + separation - 5, Y = top + 17, Width = 10, Height = 8, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });

        // Crab trolley riding on top of both rails
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - separation - 15,
            Y = top - 2,
            Width = separation * 2 + 30,
            Height = 18,
            Fill = MachineryFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Crab trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = cx - separation, CenterY = top + 16, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = cx + separation, CenterY = top + 16, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist unit hanging between the girders
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 18,
            Y = top + 16,
            Width = 36,
            Height = 35,
            Fill = DarkSteel,
            Stroke = "#154360",
            StrokeWidth = 1.2
        });

        // Hook drop
        AddHook(drawing, cx, top + 51, 40, 2);

        // Centerline
        AddCenterline(drawing, cx, top - 15, cx, top + boxH + 75);
    }
}
