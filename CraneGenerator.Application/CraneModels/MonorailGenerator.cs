using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Monorail crane system:
/// Single overhead steel I-beam track suspended from building roof structure.
/// Electric chain or wire rope hoist traveling along the lower flange.
/// </summary>
public sealed class MonorailGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 65;
        var right = view.X + view.Width - 65;
        var ground = view.Y + view.Height - 32;
        var ceilingY = view.Y + 68;
        var trackY = ceilingY + 38;
        var hoistX = HoistX(left + 45, right - 45, calculation.HoistPositionRatio);

        // Ground line
        AddGround(drawing, left - 25, right + 25, ground);

        // Ceiling / roof support beam
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 15, Y = ceilingY - 12,
            Width = right - left + 30, Height = 12,
            Fill = DarkSteel, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Vertical suspension hangers / drop rods
        for (var hx = left + 40; hx <= right - 40; hx += (right - left - 80) / 3)
        {
            drawing.Elements.Add(new DrawingRectangle
            {
                X = hx - 4, Y = ceilingY,
                Width = 8, Height = trackY - ceilingY,
                Fill = SteelFill, Stroke = "#333333", StrokeWidth = 1.2
            });
            // Hanger bracket clamp
            drawing.Elements.Add(new DrawingRectangle
            {
                X = hx - 9, Y = trackY - 6,
                Width = 18, Height = 6,
                Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1
            });
        }

        // Monorail I-beam track (single beam)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left, Y = trackY,
            Width = right - left, Height = 22,
            Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2
        });

        // End stops at both beam extremities
        drawing.Elements.Add(new DrawingRectangle { X = left - 6, Y = trackY - 6, Width = 6, Height = 34, Fill = "#e74c3c", Stroke = "#922b21", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingRectangle { X = right, Y = trackY - 6, Width = 6, Height = 34, Fill = "#e74c3c", Stroke = "#922b21", StrokeWidth = 1 });

        // Centerline on track
        AddCenterline(drawing, left - 15, trackY + 11, right + 15, trackY + 11);

        // Underslung hoist trolley (grips bottom flange)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 25, Y = trackY + 22,
            Width = 50, Height = 22,
            Fill = HoistFill, Stroke = "#1b4f72", StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 14, CenterY = trackY + 22, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 14, CenterY = trackY + 22, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Electric hoist body
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 14, Y = trackY + 44,
            Width = 28, Height = 20,
            Fill = MachineryFill, Stroke = "#154360", StrokeWidth = 1.2
        });

        // Hook drop
        AddHook(drawing, hoistX, trackY + 64, calculation.LiftRopeLength, 2);

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = trackY - 8,
            Content = "MONORAIL I-BEAM TRACK",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = left + 15, Y = ceilingY - 18,
            Content = "OVERHEAD ROOF STRUCTURE",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#555555"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 55;
        var right = view.X + view.Width - 55;
        var centerY = view.Y + view.Height / 2;
        var hoistX = HoistX(left + 45, right - 45, calculation.HoistPositionRatio);

        // Single straight monorail track line
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left, Y1 = centerY,
            X2 = right, Y2 = centerY,
            Stroke = GirderFill, StrokeWidth = 8
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left, Y1 = centerY,
            X2 = right, Y2 = centerY,
            Stroke = "#d4ac0d", StrokeWidth = 1.5
        });

        // Centerline
        AddCenterline(drawing, left - 15, centerY, right + 15, centerY);

        // Suspension mounting bracket plates along track
        for (var hx = left + 40; hx <= right - 40; hx += (right - left - 80) / 3)
        {
            drawing.Elements.Add(new DrawingRectangle
            {
                X = hx - 12, Y = centerY - 15,
                Width = 24, Height = 30,
                Fill = SteelFill, Stroke = "#333333", StrokeWidth = 1
            });
        }

        // Hoist trolley footprint in plan
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 22, Y = centerY - 18,
            Width = 44, Height = 36,
            Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.8
        });

        // Longitudinal travel arrows
        drawing.Elements.Add(new DrawingLine { X1 = hoistX + 35, Y1 = centerY, X2 = hoistX + 60, Y2 = centerY, Stroke = "#27ae60", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = hoistX - 35, Y1 = centerY, X2 = hoistX - 60, Y2 = centerY, Stroke = "#27ae60", StrokeWidth = 2 });

        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = centerY - 28,
            Content = "TRACK TRAVEL PATH",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#27ae60"
        });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var cx = view.X + view.Width / 2;
        var top = view.Y + 45;

        // Ceiling beam cross section
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 50, Y = top,
            Width = 100, Height = 14,
            Fill = DarkSteel, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Vertical hanger rod
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 5, Y = top + 14,
            Width = 10, Height = 25,
            Fill = SteelFill, Stroke = "#333333", StrokeWidth = 1.2
        });

        // I-beam Profile: Top flange, web, bottom flange
        const double flangeW = 55;
        const double flangeH = 8;
        const double webH = 40;
        const double webW = 10;

        // Top flange
        drawing.Elements.Add(new DrawingRectangle { X = cx - flangeW / 2, Y = top + 39, Width = flangeW, Height = flangeH, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 1.5 });
        // Web
        drawing.Elements.Add(new DrawingRectangle { X = cx - webW / 2, Y = top + 39 + flangeH, Width = webW, Height = webH, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 1.5 });
        // Bottom flange
        drawing.Elements.Add(new DrawingRectangle { X = cx - flangeW / 2, Y = top + 39 + flangeH + webH, Width = flangeW, Height = flangeH, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 1.5 });

        // Trolley side plates & wheels riding bottom flange
        var bFlangeY = top + 39 + flangeH + webH;
        drawing.Elements.Add(new DrawingCircle { CenterX = cx - flangeW / 2 + 5, CenterY = bFlangeY, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = cx + flangeW / 2 - 5, CenterY = bFlangeY, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Suspended hoist unit
        drawing.Elements.Add(new DrawingRectangle
        {
            X = cx - 22, Y = bFlangeY + flangeH + 4,
            Width = 44, Height = 34,
            Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5
        });

        // Hook drop
        AddHook(drawing, cx, bFlangeY + flangeH + 38, 30, 1.5);

        // Centerline
        AddCenterline(drawing, cx, top - 15, cx, bFlangeY + 95);
    }
}
