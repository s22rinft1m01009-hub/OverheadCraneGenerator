using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Underslung overhead crane:
/// Operates beneath the building roof runway structure.
/// End trucks are suspended under the bottom flange of overhead runway beams.
/// Maximizes building width utilization and side hook approach.
/// </summary>
public sealed class UnderslungGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 65;
        var right = view.X + view.Width - 65;
        var ground = view.Y + view.Height - 32;
        var top = view.Y + 65;
        var runwayBeamY = top + 25;
        var bridgeGirderY = runwayBeamY + 36;
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Ground line
        AddGround(drawing, left - 15, right + 15, ground);

        // Building columns supporting the ceiling runway roof structure
        AddBuildingColumn(drawing, left - 10, top, ground, 25);
        AddBuildingColumn(drawing, right + 10, top, ground, 25);

        // Overhead runway I-beams mounted to roof structure
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 15, Y = runwayBeamY,
            Width = 35, Height = 22,
            Fill = DarkSteel, Stroke = "#222222", StrokeWidth = 1.5
        });
        drawing.Elements.Add(new DrawingRectangle
        {
            X = right - 50, Y = runwayBeamY,
            Width = 35, Height = 22,
            Fill = DarkSteel, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Underslung end trucks suspended BELOW runway beams
        AddEndTruck(drawing, left + 10, runwayBeamY + 22, 45, 14, false);
        AddEndTruck(drawing, right - 55, runwayBeamY + 22, 45, 14, false);

        // Underslung main bridge girder suspended directly beneath end trucks
        var girderLeft = left + 5;
        var girderWidth = right - left - 10;
        drawing.Elements.Add(new DrawingRectangle
        {
            X = girderLeft,
            Y = bridgeGirderY,
            Width = girderWidth,
            Height = 28,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Girder centerline
        AddCenterline(drawing, girderLeft - 10, bridgeGirderY + 14, girderLeft + girderWidth + 10, bridgeGirderY + 14);

        // Underslung trolley suspended below bridge girder
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 28,
            Y = bridgeGirderY + 28,
            Width = 56,
            Height = 24,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 16, CenterY = bridgeGirderY + 28, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 16, CenterY = bridgeGirderY + 28, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist machinery
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 12,
            Y = bridgeGirderY + 46,
            Width = 24,
            Height = 18,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1
        });

        // Wire rope & Hook
        AddHook(drawing, hoistX, bridgeGirderY + 64, calculation.LiftRopeLength, 2);

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = left + 32, Y = runwayBeamY - 8,
            Content = "OVERHEAD RUNWAY BEAMS",
            FontSize = 7, FontWeight = "bold",
            TextAnchor = "start", Fill = "#2c3e50"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = bridgeGirderY - 6,
            Content = "SUSPENDED BRIDGE GIRDER",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 45;
        var right = view.X + view.Width - 45;
        var centerY = view.Y + view.Height / 2;
        var runwayDepth = Math.Clamp(calculation.RunwayDrawingDepth, 35, 70);
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Overhead runway tracks (two parallel dashed / heavy lines)
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY - runwayDepth, X2 = right + 15, Y2 = centerY - runwayDepth, Stroke = "#34495e", StrokeWidth = 3 });
        drawing.Elements.Add(new DrawingLine { X1 = left - 15, Y1 = centerY + runwayDepth, X2 = right + 15, Y2 = centerY + runwayDepth, Stroke = "#34495e", StrokeWidth = 3 });

        // Underslung bridge girder (extends past runway tracks)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 10,
            Y = centerY - 14,
            Width = right - left + 20,
            Height = 28,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Underslung end truck assemblies in plan
        drawing.Elements.Add(new DrawingRectangle { X = left + 10, Y = centerY - runwayDepth - 12, Width = 20, Height = 24, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingRectangle { X = left + 10, Y = centerY + runwayDepth - 12, Width = 20, Height = 24, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingRectangle { X = right - 30, Y = centerY - runwayDepth - 12, Width = 20, Height = 24, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingRectangle { X = right - 30, Y = centerY + runwayDepth - 12, Width = 20, Height = 24, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.2 });

        // Centerline
        AddCenterline(drawing, left - 20, centerY, right + 20, centerY);

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
        var cx = view.X + view.Width / 2;
        var top = view.Y + 45;

        // Overhead runway I-beam (ceiling mount)
        const double rFlangeW = 60;
        drawing.Elements.Add(new DrawingRectangle { X = cx - rFlangeW / 2, Y = top, Width = rFlangeW, Height = 8, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingRectangle { X = cx - 5, Y = top + 8, Width = 10, Height = 30, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1.2 });
        drawing.Elements.Add(new DrawingRectangle { X = cx - rFlangeW / 2, Y = top + 38, Width = rFlangeW, Height = 8, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1.2 });

        // Underslung wheels clamping bottom flange
        drawing.Elements.Add(new DrawingCircle { CenterX = cx - rFlangeW / 2 + 6, CenterY = top + 36, Radius = 6, Fill = SteelFill, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = cx + rFlangeW / 2 - 6, CenterY = top + 36, Radius = 6, Fill = SteelFill, Stroke = "#111111", StrokeWidth = 1 });

        // Suspension bracket / end truck yoke
        drawing.Elements.Add(new DrawingRectangle { X = cx - 35, Y = top + 46, Width = 70, Height = 18, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Suspended main bridge girder cross-section
        drawing.Elements.Add(new DrawingRectangle { X = cx - 25, Y = top + 64, Width = 50, Height = 35, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2 });

        // Underslung trolley & hoist below bridge girder
        drawing.Elements.Add(new DrawingRectangle { X = cx - 20, Y = top + 99, Width = 40, Height = 25, Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5 });
        AddHook(drawing, cx, top + 124, 25, 1.5);

        // Centerline
        AddCenterline(drawing, cx, top - 15, cx, top + 155);
    }
}
