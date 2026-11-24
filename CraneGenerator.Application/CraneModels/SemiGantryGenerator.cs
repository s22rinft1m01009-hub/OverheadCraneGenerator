using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Semi-gantry crane:
/// Hybrid structure with one side elevated on building runway beam and columns,
/// and the opposite side freestanding with an A-frame leg traveling on ground rail.
/// </summary>
public sealed class SemiGantryGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 65;
        var right = view.X + view.Width - 75;
        var ground = view.Y + view.Height - 32;
        var top = view.Y + 68;
        var colW = 28;
        var railY = top + 35;
        var girderTopY = railY - 26;
        var girderLeft = left + 14;
        var girderWidth = right - left - 10;
        var hoistX = HoistX(girderLeft + 35, girderLeft + girderWidth - 45, calculation.HoistPositionRatio);

        // Ground line across whole width
        AddGround(drawing, left - 15, right + 45, ground);

        // Left side: Building column with elevated corbel (EOT style)
        AddBuildingColumn(drawing, left, top, ground, colW);
        AddRunwayRail(drawing, left + 14, railY - 10, 20, 10);
        AddEndTruck(drawing, left + 2, girderTopY - 10, 55, 18, true);

        // Right side: Freestanding inclined gantry leg down to ground (Gantry style)
        drawing.Elements.Add(new DrawingLine
        {
            X1 = right, Y1 = girderTopY + 30,
            X2 = right + 32, Y2 = ground - 12,
            Stroke = "#34495e", StrokeWidth = 3.5
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = right - 10, Y1 = girderTopY + 30,
            X2 = right + 18, Y2 = ground - 12,
            Stroke = "#34495e", StrokeWidth = 2
        });

        // Right side ground rail and sill beam
        drawing.Elements.Add(new DrawingRectangle { X = right + 10, Y = ground - 6, Width = 35, Height = 6, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingRectangle
        {
            X = right + 5, Y = ground - 12,
            Width = 55, Height = 12,
            Fill = SteelFill, Stroke = "#222222", StrokeWidth = 2
        });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 18, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 48, CenterY = ground - 6, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Main bridge girder connecting elevated end truck to ground leg
        drawing.Elements.Add(new DrawingRectangle
        {
            X = girderLeft,
            Y = girderTopY,
            Width = girderWidth,
            Height = 32,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Girder centerline
        AddCenterline(drawing, girderLeft - 10, girderTopY + 16, girderLeft + girderWidth + 10, girderTopY + 16);

        // Trolley
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 30,
            Y = girderTopY + 32,
            Width = 60,
            Height = 26,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 16, CenterY = girderTopY + 32, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 16, CenterY = girderTopY + 32, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist body
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 14,
            Y = girderTopY + 48,
            Width = 28,
            Height = 18,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1
        });

        // Hook drop
        AddHook(drawing, hoistX, girderTopY + 66, calculation.LiftRopeLength, 2);

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = left + 20, Y = top + 20,
            Content = "ELEVATED RUNWAY",
            FontSize = 7, FontWeight = "bold",
            TextAnchor = "start", Fill = "#2c3e50"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = right + 20, Y = ground - 20,
            Content = "GROUND RAIL",
            FontSize = 7, FontWeight = "bold",
            TextAnchor = "start", Fill = "#2c3e50"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 45;
        var right = view.X + view.Width - 45;
        var centerY = view.Y + view.Height / 2;
        var runwayDepth = Math.Clamp(calculation.RunwayDrawingDepth, 35, 70);
        var hoistX = HoistX(left + 35, right - 35, calculation.HoistPositionRatio);

        // Top: Elevated building runway rail
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY - runwayDepth, X2 = right + 15, Y2 = centerY - runwayDepth, Stroke = "#2c3e50", StrokeWidth = 3.5 });

        // Bottom: Ground-level gantry rail
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY + runwayDepth, X2 = right + 15, Y2 = centerY + runwayDepth, Stroke = "#34495e", StrokeWidth = 3.5 });

        // Left elevated end truck
        drawing.Elements.Add(new DrawingRectangle { X = left - 10, Y = centerY - runwayDepth - 6, Width = 20, Height = 45, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.2 });

        // Right ground sill carriage
        drawing.Elements.Add(new DrawingRectangle { X = right - 5, Y = centerY + runwayDepth - 25, Width = 25, Height = 55, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Main bridge girder
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 10,
            Y = centerY - 14,
            Width = right - left - 10,
            Height = 28,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Centerline
        AddCenterline(drawing, left, centerY, right, centerY);

        // Trolley in plan
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = hoistX,
            CenterY = centerY,
            Radius = 16,
            Fill = MachineryFill,
            Stroke = "#1b4f72",
            StrokeWidth = 1.5
        });

        // Travel direction
        var arrowX = (left + right) / 2;
        drawing.Elements.Add(new DrawingLine { X1 = arrowX, Y1 = centerY - runwayDepth + 12, X2 = arrowX, Y2 = centerY - runwayDepth + 26, Stroke = "#27ae60", StrokeWidth = 2 });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var left = view.X + 45;
        var right = view.X + view.Width - 45;
        var centerY = view.Y + view.Height / 2;

        // Elevated column on left
        drawing.Elements.Add(new DrawingRectangle { X = left - 10, Y = centerY - 65, Width = 18, Height = 95, Fill = ColumnFill, Stroke = "#333333", StrokeWidth = 1.5 });
        drawing.Elements.Add(new DrawingRectangle { X = left + 8, Y = centerY - 35, Width = 14, Height = 14, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Elevated runway rail & wheel
        drawing.Elements.Add(new DrawingRectangle { X = left + 10, Y = centerY - 43, Width = 10, Height = 8, Fill = "#7f8c8d", Stroke = "#2c3e50", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = left + 15, CenterY = centerY - 47, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Bridge girder spanning across
        drawing.Elements.Add(new DrawingRectangle { X = left + 20, Y = centerY - 52, Width = right - left - 40, Height = 22, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2 });

        // Freestanding leg descending on right to ground
        drawing.Elements.Add(new DrawingLine { X1 = right - 20, Y1 = centerY - 30, X2 = right + 5, Y2 = centerY + 45, Stroke = "#34495e", StrokeWidth = 3 });
        drawing.Elements.Add(new DrawingRectangle { X = right - 10, Y = centerY + 45, Width = 30, Height = 10, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Trolley & hook
        drawing.Elements.Add(new DrawingRectangle { X = (left + right) / 2 - 18, Y = centerY - 30, Width = 36, Height = 25, Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5 });
        AddHook(drawing, (left + right) / 2, centerY - 5, 30, 1.5);

        // Ground line below right leg
        AddGround(drawing, right - 25, right + 35, centerY + 55);
    }
}
