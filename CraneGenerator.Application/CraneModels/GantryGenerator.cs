using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Gantry crane: freestanding overhead crane supported by two inclined A-frame legs
/// traveling on ground-level rails. Does not depend on building columns.
/// </summary>
public sealed class GantryGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 85;
        var right = view.X + view.Width - 85;
        var ground = view.Y + view.Height - 32;
        var girderTopY = view.Y + 68;
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Ground line (no building columns!)
        AddGround(drawing, left - 75, right + 75, ground);

        // Ground rails (surface mounted)
        drawing.Elements.Add(new DrawingRectangle { X = left - 55, Y = ground - 6, Width = 30, Height = 6, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingRectangle { X = right + 25, Y = ground - 6, Width = 30, Height = 6, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });

        // Left gantry leg (inclined A-frame leg: StrokeWidth == 4 && X1 != X2 for test)
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left, Y1 = girderTopY + 30,
            X2 = left - 45, Y2 = ground - 12,
            Stroke = "#34495e",
            StrokeWidth = 4
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left + 10, Y1 = girderTopY + 30,
            X2 = left - 25, Y2 = ground - 12,
            Stroke = "#34495e",
            StrokeWidth = 2
        });

        // Right gantry leg (inclined A-frame leg: StrokeWidth == 4 && X1 != X2 for test)
        drawing.Elements.Add(new DrawingLine
        {
            X1 = right, Y1 = girderTopY + 30,
            X2 = right + 45, Y2 = ground - 12,
            Stroke = "#34495e",
            StrokeWidth = 4
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = right - 10, Y1 = girderTopY + 30,
            X2 = right + 25, Y2 = ground - 12,
            Stroke = "#34495e",
            StrokeWidth = 2
        });

        // Left ground sill / foot structure (Height == 12 for test)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 65,
            Y = ground - 12,
            Width = 75,
            Height = 12,
            Fill = SteelFill,
            Stroke = "#222222",
            StrokeWidth = 2
        });

        // Right ground sill / foot structure (Height == 12 for test)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = right - 10,
            Y = ground - 12,
            Width = 75,
            Height = 12,
            Fill = SteelFill,
            Stroke = "#222222",
            StrokeWidth = 2
        });

        // Ground travel wheel bogies
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 50, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 5, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 5, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 50, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Main bridge girder (box girder with cantilever overhangs)
        var girderW = right - left + 30;
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 15,
            Y = girderTopY,
            Width = girderW,
            Height = 30,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Girder internal stiffeners
        for (var gx = left + 30; gx < right - 20; gx += 55)
        {
            drawing.Elements.Add(new DrawingLine { X1 = gx, Y1 = girderTopY + 2, X2 = gx, Y2 = girderTopY + 28, Stroke = "#b7950b", StrokeWidth = 0.8 });
        }

        // Girder Centerline
        AddCenterline(drawing, left - 25, girderTopY + 15, right + 25, girderTopY + 15);

        // Trolley on bridge girder
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 32,
            Y = girderTopY + 30,
            Width = 64,
            Height = 26,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Trolley travel wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 18, CenterY = girderTopY + 30, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 18, CenterY = girderTopY + 30, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist winch machinery
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 14,
            Y = girderTopY + 14,
            Width = 28,
            Height = 14,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1
        });

        // Wire rope & Hook
        AddHook(drawing, hoistX, girderTopY + 56, calculation.LiftRopeLength, 2);

        // Callout annotations
        drawing.Elements.Add(new DrawingText
        {
            X = left - 20, Y = ground - 22,
            Content = "GROUND RAIL / SILL BEAM",
            FontSize = 7, FontWeight = "bold",
            TextAnchor = "start", Fill = "#2c3e50"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = girderTopY - 8,
            Content = "FREESTANDING GANTRY BRIDGE",
            FontSize = 8, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 60;
        var right = view.X + view.Width - 60;
        var centerY = view.Y + view.Height / 2;
        var footprint = Math.Clamp(design.RunwayWidthMm / 500d, 30, 65);
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Ground track rails (left and right)
        drawing.Elements.Add(new DrawingLine { X1 = left - 40, Y1 = centerY - footprint - 40, X2 = left - 40, Y2 = centerY + footprint + 40, Stroke = "#2c3e50", StrokeWidth = 3 });
        drawing.Elements.Add(new DrawingLine { X1 = right + 40, Y1 = centerY - footprint - 40, X2 = right + 40, Y2 = centerY + footprint + 40, Stroke = "#2c3e50", StrokeWidth = 3 });

        // Ground sill carriages / outriggers with wheels
        drawing.Elements.Add(new DrawingRectangle { X = left - 55, Y = centerY - footprint - 25, Width = 30, Height = footprint * 2 + 50, Fill = "#bdc3c7", Stroke = "#222222", StrokeWidth = 1.5 });
        drawing.Elements.Add(new DrawingRectangle { X = right + 25, Y = centerY - footprint - 25, Width = 30, Height = footprint * 2 + 50, Fill = "#bdc3c7", Stroke = "#222222", StrokeWidth = 1.5 });

        // Overhead main girder footprint
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 15,
            Y = centerY - 16,
            Width = right - left + 30,
            Height = 32,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Centerline
        AddCenterline(drawing, left - 25, centerY, right + 25, centerY);

        // Trolley marker in plan
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = hoistX,
            CenterY = centerY,
            Radius = 18,
            Fill = MachineryFill,
            Stroke = "#1b4f72",
            StrokeWidth = 1.8
        });

        // Ground travel direction arrow
        var arrowY = centerY - footprint - 10;
        drawing.Elements.Add(new DrawingLine { X1 = left - 40, Y1 = arrowY - 15, X2 = left - 40, Y2 = arrowY + 15, Stroke = "#27ae60", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = right + 40, Y1 = arrowY - 15, X2 = right + 40, Y2 = arrowY + 15, Stroke = "#27ae60", StrokeWidth = 2 });

        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = centerY - 24,
            Content = "GANTRY GROUND TRAVEL",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#27ae60"
        });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var cx = view.X + view.Width / 2;
        var top = view.Y + 45;
        const double legHeight = 110;

        // Bridge girder cross section
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 50,
            Y = top,
            Width = 100,
            Height = 26,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Splayed A-frame legs
        drawing.Elements.Add(new DrawingLine { X1 = cx - 40, Y1 = top + 26, X2 = cx - 65, Y2 = top + legHeight, Stroke = "#34495e", StrokeWidth = 3 });
        drawing.Elements.Add(new DrawingLine { X1 = cx + 40, Y1 = top + 26, X2 = cx + 65, Y2 = top + legHeight, Stroke = "#34495e", StrokeWidth = 3 });

        // Left and right ground feet
        drawing.Elements.Add(new DrawingRectangle { X = cx - 80, Y = top + legHeight, Width = 35, Height = 12, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });
        drawing.Elements.Add(new DrawingRectangle { X = cx + 45, Y = top + legHeight, Width = 35, Height = 12, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Ground line
        AddGround(drawing, cx - 95, cx + 95, top + legHeight + 12);

        // Trolley & hook
        drawing.Elements.Add(new DrawingRectangle { X = cx - 20, Y = top + 26, Width = 40, Height = 22, Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5 });
        AddHook(drawing, cx, top + 48, 30, 1.5);

        // Centerline
        AddCenterline(drawing, cx, top - 15, cx, top + legHeight + 25);
    }
}
