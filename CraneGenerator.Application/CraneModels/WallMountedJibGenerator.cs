using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Wall-mounted Jib Crane:
/// Mounted directly to building structural columns or concrete walls.
/// Cantilever boom supported by upper/lower hinge brackets with diagonal tie rod.
/// Provides 180° rotation coverage without taking floor space.
/// </summary>
public sealed class WallMountedJibGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 85;
        var right = view.X + view.Width - 75;
        var ground = view.Y + view.Height - 32;
        var top = view.Y + 65;
        var wallW = 28;
        var wallX = left + 14;
        var boomY = top + 55;
        var boomLength = right - wallX - 35;
        var hoistX = HoistX(wallX + 35, wallX + boomLength - 20, calculation.HoistPositionRatio);

        // Ground line
        AddGround(drawing, left - 15, right + 15, ground);

        // Heavy concrete/steel building wall
        drawing.Elements.Add(new DrawingRectangle
        {
            X = wallX - wallW / 2,
            Y = top,
            Width = wallW,
            Height = ground - top,
            Fill = ColumnFill,
            Stroke = "#333333",
            StrokeWidth = 2
        });

        // Upper wall hinge bracket & backer plate
        drawing.Elements.Add(new DrawingRectangle
        {
            X = wallX + wallW / 2,
            Y = top + 15,
            Width = 16,
            Height = 22,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.5
        });

        // Lower wall hinge bracket & backer plate
        drawing.Elements.Add(new DrawingRectangle
        {
            X = wallX + wallW / 2,
            Y = boomY + 20,
            Width = 18,
            Height = 24,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.5
        });

        // Wall anchor through-bolts
        drawing.Elements.Add(new DrawingLine { X1 = wallX - wallW / 2 - 8, Y1 = top + 26, X2 = wallX + wallW / 2, Y2 = top + 26, Stroke = "#111111", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = wallX - wallW / 2 - 8, Y1 = boomY + 32, X2 = wallX + wallW / 2, Y2 = boomY + 32, Stroke = "#111111", StrokeWidth = 2 });

        // Diagonal tie-rod connecting upper wall hinge to boom
        drawing.Elements.Add(new DrawingLine
        {
            X1 = wallX + wallW / 2 + 16,
            Y1 = top + 26,
            X2 = wallX + wallW / 2 + boomLength * 0.7,
            Y2 = boomY,
            Stroke = "#1b4f72",
            StrokeWidth = 2.5
        });

        // Cantilever horizontal boom girder
        drawing.Elements.Add(new DrawingRectangle
        {
            X = wallX + wallW / 2 + 16,
            Y = boomY,
            Width = boomLength,
            Height = 24,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Boom end stop
        drawing.Elements.Add(new DrawingRectangle
        {
            X = wallX + wallW / 2 + 16 + boomLength - 6,
            Y = boomY - 4,
            Width = 6,
            Height = 32,
            Fill = "#e74c3c",
            Stroke = "#922b21",
            StrokeWidth = 1
        });

        // Boom horizontal centerline
        AddCenterline(drawing, wallX + wallW / 2 + 16, boomY + 12, wallX + wallW / 2 + 16 + boomLength + 10, boomY + 12);

        // Underslung trolley
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 24,
            Y = boomY + 24,
            Width = 48,
            Height = 22,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 14, CenterY = boomY + 24, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 14, CenterY = boomY + 24, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist machinery
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 12,
            Y = boomY + 46,
            Width = 24,
            Height = 18,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1
        });

        // Hook drop
        AddHook(drawing, hoistX, boomY + 64, calculation.LiftRopeLength, 2);

        // Swing rotation indicator arc (180° swing)
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = wallX + wallW / 2,
            CenterY = boomY + 12,
            Radius = 40,
            StartAngleDegrees = 270,
            EndAngleDegrees = 90,
            Stroke = "#7f8c8d",
            StrokeWidth = 1
        });

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = wallX, Y = top - 8,
            Content = "BUILDING WALL / COLUMN",
            FontSize = 7, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#2c3e50"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = wallX + wallW / 2 + boomLength / 2, Y = boomY - 8,
            Content = "WALL JIB BOOM (180° SWING)",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 65;
        var centerY = view.Y + view.Height / 2;
        const double sweepRadius = 110;
        const double boomAngleDeg = 25;
        var boomRad = boomAngleDeg * Math.PI / 180.0;
        var boomEndX = left + sweepRadius * Math.Cos(boomRad);
        var boomEndY = centerY - sweepRadius * Math.Sin(boomRad);

        // Building wall face in plan (thick vertical line on left)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 18, Y = centerY - sweepRadius - 15,
            Width = 18, Height = (sweepRadius + 15) * 2,
            Fill = ColumnFill, Stroke = "#333333", StrokeWidth = 2
        });

        // 180° semi-circular swing envelope boundary (dashed arc)
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = left, CenterY = centerY,
            Radius = sweepRadius,
            StartAngleDegrees = 270, EndAngleDegrees = 90,
            Stroke = "#95a5a6", StrokeWidth = 1.2
        });

        // Pivot brackets
        drawing.Elements.Add(new DrawingCircle { CenterX = left, CenterY = centerY, Radius = 8, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1.5 });

        // Cantilever boom swung out into bay
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left, Y1 = centerY,
            X2 = boomEndX, Y2 = boomEndY,
            Stroke = GirderFill, StrokeWidth = 7
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left, Y1 = centerY,
            X2 = boomEndX, Y2 = boomEndY,
            Stroke = "#d4ac0d", StrokeWidth = 1.5
        });

        // Trolley on boom
        var trolleyDist = sweepRadius * Math.Clamp(calculation.HoistPositionRatio, 0.25, 0.9);
        var tx = left + trolleyDist * Math.Cos(boomRad);
        var ty = centerY - trolleyDist * Math.Sin(boomRad);

        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = tx, CenterY = ty,
            Radius = 14, Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5
        });

        // 180° swing rotation arrows
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = left, CenterY = centerY,
            Radius = sweepRadius + 12,
            StartAngleDegrees = 300, EndAngleDegrees = 60,
            Stroke = "#27ae60", StrokeWidth = 1.5
        });

        drawing.Elements.Add(new DrawingText
        {
            X = left + sweepRadius + 18, Y = centerY - 25,
            Content = "180° SWING ARC",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#27ae60"
        });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var left = view.X + 45;
        var top = view.Y + 45;

        // Wall cross-section
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left, Y = top,
            Width = 22, Height = 140,
            Fill = ColumnFill, Stroke = "#333333", StrokeWidth = 1.5
        });

        // Mounting bracket & hinge pin
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 22, Y = top + 35,
            Width = 16, Height = 25,
            Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1.2
        });
        drawing.Elements.Add(new DrawingCircle { CenterX = left + 30, CenterY = top + 47, Radius = 4, Fill = SteelFill, Stroke = "#111111", StrokeWidth = 1 });

        // Cantilever boom cross-section extending to right
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 38, Y = top + 37,
            Width = 90, Height = 20,
            Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2
        });

        // Underslung trolley
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left + 65, Y = top + 57,
            Width = 36, Height = 26,
            Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5
        });

        // Hook drop
        AddHook(drawing, left + 83, top + 83, 28, 1.5);

        // Centerline
        AddCenterline(drawing, left + 83, top + 20, left + 83, top + 130);
    }
}
