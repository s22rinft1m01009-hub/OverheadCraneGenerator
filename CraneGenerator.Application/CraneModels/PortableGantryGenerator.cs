using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Domain.Entities;
using System;

namespace CraneGenerator.Application.CraneModels;

/// <summary>
/// Portable A-frame Gantry Crane:
/// Light/medium duty mobile floor crane on 4 heavy-duty industrial swivel casters.
/// Splayed tubular legs with diagonal corner gussets and underslung hoist.
/// </summary>
public sealed class PortableGantryGenerator : CraneGeometryGeneratorBase
{
    protected override void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[0];
        var left = view.X + 90;
        var right = view.X + view.Width - 90;
        var ground = view.Y + view.Height - 32;
        var beamY = view.Y + 70;
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Workshop floor line (no rails)
        AddGround(drawing, left - 40, right + 40, ground);

        // Top horizontal cross beam
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 15,
            Y = beamY,
            Width = right - left + 30,
            Height = 24,
            Fill = GirderFill,
            Stroke = "#d4ac0d",
            StrokeWidth = 2
        });

        // Beam centerline
        AddCenterline(drawing, left - 25, beamY + 12, right + 25, beamY + 12);

        // Diagonal corner gusset braces
        drawing.Elements.Add(new DrawingLine { X1 = left + 35, Y1 = beamY + 24, X2 = left - 5, Y2 = beamY + 55, Stroke = "#34495e", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = right - 35, Y1 = beamY + 24, X2 = right + 5, Y2 = beamY + 55, Stroke = "#34495e", StrokeWidth = 2 });

        // Left tubular A-frame leg
        drawing.Elements.Add(new DrawingLine { X1 = left, Y1 = beamY + 24, X2 = left - 28, Y2 = ground - 14, Stroke = "#34495e", StrokeWidth = 3.5 });
        drawing.Elements.Add(new DrawingLine { X1 = left + 8, Y1 = beamY + 24, X2 = left - 12, Y2 = ground - 14, Stroke = "#34495e", StrokeWidth = 2 });

        // Right tubular A-frame leg
        drawing.Elements.Add(new DrawingLine { X1 = right, Y1 = beamY + 24, X2 = right + 28, Y2 = ground - 14, Stroke = "#34495e", StrokeWidth = 3.5 });
        drawing.Elements.Add(new DrawingLine { X1 = right - 8, Y1 = beamY + 24, X2 = right + 12, Y2 = ground - 14, Stroke = "#34495e", StrokeWidth = 2 });

        // Left base outrigger sill channel
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 45, Y = ground - 14,
            Width = 50, Height = 10,
            Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Right base outrigger sill channel
        drawing.Elements.Add(new DrawingRectangle
        {
            X = right - 5, Y = ground - 14,
            Width = 50, Height = 10,
            Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Swivel caster wheels with brake levers
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 35, CenterY = ground - 5, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 5, CenterY = ground - 5, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 5, CenterY = ground - 5, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 35, CenterY = ground - 5, Radius = 5, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Push handles on legs
        drawing.Elements.Add(new DrawingLine { X1 = left - 25, Y1 = ground - 55, X2 = left - 38, Y2 = ground - 55, Stroke = "#e74c3c", StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingLine { X1 = right + 25, Y1 = ground - 55, X2 = right + 38, Y2 = ground - 55, Stroke = "#e74c3c", StrokeWidth = 2 });

        // Underslung trolley
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 25, Y = beamY + 24,
            Width = 50, Height = 22,
            Fill = HoistFill, Stroke = "#1b4f72", StrokeWidth = 2
        });

        // Trolley wheels
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX - 14, CenterY = beamY + 24, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = hoistX + 14, CenterY = beamY + 24, Radius = 4, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Hoist body
        drawing.Elements.Add(new DrawingRectangle
        {
            X = hoistX - 12, Y = beamY + 46,
            Width = 24, Height = 18,
            Fill = MachineryFill, Stroke = "#154360", StrokeWidth = 1
        });

        // Hook drop
        AddHook(drawing, hoistX, beamY + 64, calculation.LiftRopeLength, 2);

        // Annotations
        drawing.Elements.Add(new DrawingText
        {
            X = (left + right) / 2, Y = beamY - 8,
            Content = "PORTABLE GANTRY TOP BEAM",
            FontSize = 7.5, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#7d6608"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = left - 35, Y = ground - 20,
            Content = "SWIVEL CASTERS",
            FontSize = 6.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#2c3e50"
        });
    }

    protected override void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[1];
        var left = view.X + 65;
        var right = view.X + view.Width - 65;
        var centerY = view.Y + view.Height / 2;
        const double footDepth = 48;
        var hoistX = HoistX(left + 25, right - 25, calculation.HoistPositionRatio);

        // Top beam footprint
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 12, Y = centerY - 10,
            Width = right - left + 24, Height = 20,
            Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 2
        });

        // Centerline
        AddCenterline(drawing, left - 25, centerY, right + 25, centerY);

        // Left outrigger channel footprint (perpendicular to beam)
        drawing.Elements.Add(new DrawingRectangle
        {
            X = left - 25, Y = centerY - footDepth,
            Width = 20, Height = footDepth * 2,
            Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5
        });

        // Right outrigger channel footprint
        drawing.Elements.Add(new DrawingRectangle
        {
            X = right + 5, Y = centerY - footDepth,
            Width = 20, Height = footDepth * 2,
            Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5
        });

        // 4 Swivel casters at outrigger corners in plan
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 15, CenterY = centerY - footDepth + 8, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = left - 15, CenterY = centerY + footDepth - 8, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 15, CenterY = centerY - footDepth + 8, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = right + 15, CenterY = centerY + footDepth - 8, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Trolley in plan
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = hoistX, CenterY = centerY,
            Radius = 15, Fill = MachineryFill, Stroke = "#1b4f72", StrokeWidth = 1.5
        });
    }

    protected override void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var view = drawing.Views[2];
        var cx = view.X + view.Width / 2;
        var top = view.Y + 45;
        const double legH = 110;

        // Beam cross section
        drawing.Elements.Add(new DrawingRectangle { X = cx - 25, Y = top, Width = 50, Height = 20, Fill = GirderFill, Stroke = "#d4ac0d", StrokeWidth = 1.5 });

        // A-frame leg spread in section
        drawing.Elements.Add(new DrawingLine { X1 = cx - 15, Y1 = top + 20, X2 = cx - 45, Y2 = top + legH, Stroke = "#34495e", StrokeWidth = 3 });
        drawing.Elements.Add(new DrawingLine { X1 = cx + 15, Y1 = top + 20, X2 = cx + 45, Y2 = top + legH, Stroke = "#34495e", StrokeWidth = 3 });

        // Horizontal bottom channel
        drawing.Elements.Add(new DrawingRectangle { X = cx - 55, Y = top + legH, Width = 110, Height = 10, Fill = SteelFill, Stroke = "#222222", StrokeWidth = 1.5 });

        // Casters on floor
        drawing.Elements.Add(new DrawingCircle { CenterX = cx - 45, CenterY = top + legH + 16, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });
        drawing.Elements.Add(new DrawingCircle { CenterX = cx + 45, CenterY = top + legH + 16, Radius = 6, Fill = DarkSteel, Stroke = "#111111", StrokeWidth = 1 });

        // Floor line
        AddGround(drawing, cx - 65, cx + 65, top + legH + 22);

        // Hook drop
        AddHook(drawing, cx, top + 42, 28, 1.5);

        // Centerline
        AddCenterline(drawing, cx, top - 15, cx, top + legH + 30);
    }
}
