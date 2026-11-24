using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Pillar-mounted Jib Crane:
/// Vertical freestanding steel column with heavy foundation base.
/// Horizontal cantilever boom with slewing bearing and underslung hoist.
/// </summary>
public sealed class JibCraneGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 90;
        var right = view.X + view.Width - 75;
        var ground = view.Y + view.Height - 32;
        var top = view.Y + 65;
        var colX = left + 60;
        var boomY = top + 40;
        var boomLength = right - colX - 25;
        var hoistX = HoistX(colX + 35, colX + boomLength - 20, calculation.HoistPositionRatio);

        // Ground line
        AddGround(drawing, left - 20, right + 20, ground);

        // Heavy concrete foundation with anchor bolts
        AddFoundation(drawing, colX, ground, 100, 20);

        // Steel base plate with gusset stiffeners
        drawing.Elements.Add(new DrawingRectangle
        {
            X = colX - 42,
            Y = ground - 14,
            Width = 84,
            Height = 14,
            Fill = SteelFill,
            Stroke = "#222222",
            StrokeWidth = 2
        });

        // Vertical pillar / tubular column
        drawing.Elements.Add(new DrawingRectangle
        {
            X = colX - 16,
            Y = top + 15,
            Width = 32,
            Height = ground - 14 - (top + 15),
            Fill = SteelFill,
            Stroke = "#333333",
            StrokeWidth = 2
        });

        // Column vertical centerline
        AddCenterline(drawing, colX, top + 5, colX, ground + 15);

        // Slewing pivot assembly at top of column
        drawing.Elements.Add(new DrawingRectangle
        {
            X = colX - 22,
            Y = top + 15,
            Width = 44,
            Height = 25,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.5
        });

        // Top tie-rod / brace connecting column mast to boom
        drawing.Elements.Add(new DrawingLine
        {
            X1 = colX, Y1 = top + 15,
            X2 = colX + boomLength * 0.65, Y2 = boomY,
            Stroke = "#1b4f72",
            StrokeWidth = 2.5
        });

        // Horizontal cantilever boom girder
        drawing.Elements.Add(new DrawingRectangle
        {
            X = colX + 16,
            Y = boomY,
            Width = boomLength,
            Height = 26,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Boom horizontal centerline
        AddCenterline(drawing, colX + 16, boomY + 13, colX + 16 + boomLength + 10, boomY + 13);

        // Boom end stop
        drawing.Elements.Add(new DrawingRectangle
        {
            X = colX + 16 + boomLength - 6,
            Y = boomY - 4,
            Width = 6,
            Height = 34,
            Fill = "#e74c3c",
            Stroke = "#922b21",
            StrokeWidth = 1
        });

        // Underslung trolley riding on boom lower flange
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 25,
            Y = boomY + 26,
            Width = 50,
            Height = 24,
            Fill = HoistFill,
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 14, CenterY = boomY + 26, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 14, CenterY = boomY + 26, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist body
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 12,
            Y = boomY + 50,
            Width = 24,
            Height = 16,
            Fill = MachineryFill,
            Stroke = "#154360",
            StrokeWidth = 1
        });

        // Wire rope and hook
        AddHook(drawing, hoistX, boomY + 66, calculation.LiftRopeLength, 2);

        // Rotation sweep indicator arc in elevation
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = colX,
            CenterY = boomY + 13,
            Radius = 45,
            StartAngleDegrees = 0,
            EndAngleDegrees = 180,
            Stroke = "#7f8c8d",
            StrokeWidth = 1
        });

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = colX + boomLength / 2, Y = boomY - 8,
            Content = "CANTILEVER BOOM (SPAN)",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = colX - 25, Y = top + 80,
            Content = "PILLAR COLUMN",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#2c3e50"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var cx = view.X + 160;
        var cy = view.Y + view.Height / 2;
        const double sweepRadius = 95;
        const double boomAngleDeg = 35;
        var boomRad = boomAngleDeg * Math.PI / 180.0;
        var boomEndX = cx + sweepRadius * Math.Cos(boomRad);
        var boomEndY = cy - sweepRadius * Math.Sin(boomRad);

        // 360° circular sweep boundary (dashed)
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = cx, CenterY = cy,
            Radius = sweepRadius,
            StartAngleDegrees = 0, EndAngleDegrees = 360,
            Stroke = "#95a5a6", StrokeWidth = 1.2
        });

        // Slew rotation arrows along perimeter
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = cx, CenterY = cy,
            Radius = sweepRadius + 12,
            StartAngleDegrees = 10, EndAngleDegrees = 70,
            Stroke = "#27ae60", StrokeWidth = 1.5
        });

        // Pillar foundation plate in plan (octagonal or circular)
        drawing.Elements.Add(new DrawingCircle { CenterX = cx, CenterY = cy, Radius = 30, Fill = "#bdc3c7", Stroke = "#2c3e50", StrokeWidth = 1.5 });
        drawing.Elements.Add(new DrawingCircle { CenterX = cx, CenterY = cy, Radius = 16, Fill = SteelFill, Stroke = "#2c3e50", StrokeWidth = 1.5 });

        // Anchor bolt markers in plan
        for (var a = 0; a < 360; a += 60)
        {
            var rad = a * Math.PI / 180.0;
            drawing.Elements.Add(new DrawingCircle { CenterX = cx + 23 * Math.Cos(rad), CenterY = cy + 23 * Math.Sin(rad), Radius = 2.5, Fill = "#111111", Stroke = "#000000", StrokeWidth = 0.8 });
        }

        // Cantilever boom extending along angle
        drawing.Elements.Add(new DrawingLine
        {
            X1 = cx, Y1 = cy,
            X2 = boomEndX, Y2 = boomEndY,
            Stroke = GirderFill,
            StrokeWidth = 8
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = cx, Y1 = cy,
            X2 = boomEndX, Y2 = boomEndY,
            Stroke = "#d4ac0d",
            StrokeWidth = 1.5
        });

        // Trolley position on boom
        var trolleyDist = sweepRadius * Math.Clamp(calculation.HoistPositionRatio, 0.25, 0.9);
        var tx = cx + trolleyDist * Math.Cos(boomRad);
        var ty = cy - trolleyDist * Math.Sin(boomRad);

        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = tx, CenterY = ty,
            Radius = 14,
            Fill = MachineryFill,
            Stroke = "#1b4f72",
            StrokeWidth = 1.5
        });

        // Centerlines
        AddCenterline(drawing, cx - sweepRadius - 15, cy, cx + sweepRadius + 15, cy);
        AddCenterline(drawing, cx, cy - sweepRadius - 15, cx, cy + sweepRadius + 15);

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = cx + sweepRadius + 20, Y = cy - 20,
            Content = "360° SLEWING ENVELOPE",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#27ae60"
        });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var cx = view.X + 65;
        var top = view.Y + 45;
        const double colH = 130;

        // Pillar column profile
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 14, Y = top,
            Width = 28, Height = colH,
            Fill = SteelFill, Stroke = "#333333", StrokeWidth = 1.5
        });

        // Base foundation
        AddFoundation(drawing, cx, top + colH, 70, 16);

        // Cantilever boom section (I-beam profile)
        const double boomW = 95;
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx + 14, Y = top + 25,
            Width = boomW, Height = 22,
            Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2
        });

        // Underslung trolley
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx + 45, Y = top + 47,
            Width = 32, Height = 28,
            Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5
        });

        // Hook drop
        AddHook(drawing, cx + 61, top + 75, 30, 1.5);

        // Section Centerline
        AddCenterline(drawing, cx, top - 15, cx, top + colH + 20);

        drawing.Elements.Add(new DrawingText
        {
            X = cx + 50, Y = top + 15,
            Content = "SECTION A-A",
            FontSize = 8.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#c0392b"
        });
    }
}
