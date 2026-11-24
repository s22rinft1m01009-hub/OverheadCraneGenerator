using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Application.Drawing.Sheet;
using CraneGenerator.Domain.Entities;
using System;
using System.Collections.Generic;

namespace CraneGenerator.Application.CraneModels;

public abstract class CraneGeometryGeneratorBase : ICraneGeometryGenerator
{
    protected const string GirderFill = "#f4c542";
    protected const string SteelFill = "#eeeeee";
    protected const string HoistFill = "#4f81bd";
    protected const string ColumnFill = "#d3d3d3";
    protected const string HighlightFill = "#ff6b6b";
    protected const string DarkSteel = "#555555";
    protected const string MachineryFill = "#2980b9";

    public DrawingModel Generate(DesignResponse design, CraneCalculationResult calculation)
    {
        var drawing = CreateDrawing(design, calculation);
        CreateElevation(drawing, design, calculation);
        CreatePlan(drawing, design, calculation);
        CreateSection(drawing, design, calculation);
        AddDimensions(drawing, design, calculation);

        // Assemble CAD Sheet (Border, Grid, Detail Z, Runway Bay, Notes, TitleBlock, Revisions)
        DrawingSheetRenderer.AssembleSheet(drawing, design, calculation);

        return drawing;
    }

    protected abstract void CreateElevation(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation);
    protected abstract void CreatePlan(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation);
    protected abstract void CreateSection(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation);

    protected static DrawingModel CreateDrawing(DesignResponse design, CraneCalculationResult calculation)
    {
        var drawing = new DrawingModel
        {
            DrawingNumber = $"CRN-{design.Id:D3}",
            Title = "PARAMETRIC OVERHEAD CRANE DRAWING",
            Scale = calculation.DrawingScale,
            Revision = "A"
        };

        drawing.Views.Add(new DrawingView { Name = "MAIN ELEVATION", X = 45, Y = 58, Width = 755, Height = 265, Scale = calculation.DrawingScale });
        drawing.Views.Add(new DrawingView { Name = "PLAN VIEW", X = 45, Y = 335, Width = 510, Height = 220, Scale = calculation.DrawingScale });
        drawing.Views.Add(new DrawingView { Name = "SECTION A-A", X = 565, Y = 335, Width = 235, Height = 220, Scale = "1:25" });

        drawing.TechnicalData = CraneCalculationService.BuildTechnicalData(design, calculation, drawing.DrawingNumber);
        return drawing;
    }

    protected static (double Left, double Right, double Ground, double Top) ElevationBounds(DrawingView view, CraneCalculationResult calculation)
    {
        var left = view.X + 55;
        var right = view.X + view.Width - 55;
        var ground = view.Y + view.Height - 32;
        var top = ground - calculation.BuildingHeightDrawing;
        return (left, right, ground, top);
    }

    protected static double HoistX(double left, double right, double hoistPositionRatio)
    {
        return left + hoistPositionRatio * (right - left);
    }

    // Geometry Helpers

    protected static void AddBuildingColumn(DrawingModel drawing, double x, double yTop, double yBottom, double width = 25)
    {
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x - width / 2,
            Y = yTop,
            Width = width,
            Height = yBottom - yTop,
            Fill = ColumnFill,
            Stroke = "#333333",
            StrokeWidth = 1.8
        });

        // Corbel / Bracket supporting runway beam
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x > 400 ? x - width / 2 - 14 : x + width / 2,
            Y = yTop + 35,
            Width = 14,
            Height = 22,
            Fill = DarkSteel,
            Stroke = "#222222",
            StrokeWidth = 1.2
        });
    }

    protected static void AddRunwayRail(DrawingModel drawing, double x, double y, double width = 16, double height = 10)
    {
        // Runway beam segment
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x - width / 2 - 4,
            Y = y + height,
            Width = width + 8,
            Height = 12,
            Fill = SteelFill,
            Stroke = "#333333",
            StrokeWidth = 1.2
        });

        // Rail profile
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x - width / 2,
            Y = y,
            Width = width,
            Height = height,
            Fill = "#7f8c8d",
            Stroke = "#2c3e50",
            StrokeWidth = 1.2
        });
    }

    protected static void AddEndTruck(DrawingModel drawing, double x, double y, double width, double height, bool isTopRunning = true)
    {
        // End truck beam frame
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x,
            Y = y,
            Width = width,
            Height = height,
            Fill = SteelFill,
            Stroke = "#222222",
            StrokeWidth = 1.5
        });

        // End truck travel wheels
        var wheelR = height * 0.45;
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = x + width * 0.22,
            CenterY = isTopRunning ? y + height : y,
            Radius = wheelR,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.2
        });
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = x + width * 0.78,
            CenterY = isTopRunning ? y + height : y,
            Radius = wheelR,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.2
        });

        // Rubber buffers at ends
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x - 5,
            Y = y + height * 0.2,
            Width = 5,
            Height = height * 0.6,
            Fill = "#e74c3c",
            Stroke = "#922b21",
            StrokeWidth = 1
        });
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x + width,
            Y = y + height * 0.2,
            Width = 5,
            Height = height * 0.6,
            Fill = "#e74c3c",
            Stroke = "#922b21",
            StrokeWidth = 1
        });
    }

    protected static void AddHook(DrawingModel drawing, double x, double ropeTop, double ropeLength, double ropeThickness = 2)
    {
        var ropeBottom = ropeTop + Math.Max(35, ropeLength);

        // Twin wire rope lines
        drawing.Elements.Add(new DrawingLine
        {
            X1 = x - 3, Y1 = ropeTop,
            X2 = x - 3, Y2 = ropeBottom,
            Stroke = "#333333",
            StrokeWidth = ropeThickness
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = x + 3, Y1 = ropeTop,
            X2 = x + 3, Y2 = ropeBottom,
            Stroke = "#333333",
            StrokeWidth = ropeThickness
        });

        // Hook block / sheave housing
        drawing.Elements.Add(new DrawingRectangle
        {
            X = x - 12,
            Y = ropeBottom,
            Width = 24,
            Height = 16,
            Fill = DarkSteel,
            Stroke = "#111111",
            StrokeWidth = 1.5
        });

        // Sheave circle
        drawing.Elements.Add(new DrawingCircle
        {
            CenterX = x,
            CenterY = ropeBottom + 8,
            Radius = 6,
            Fill = SteelFill,
            Stroke = "#222222",
            StrokeWidth = 1
        });

        // Hook shank
        drawing.Elements.Add(new DrawingLine
        {
            X1 = x, Y1 = ropeBottom + 16,
            X2 = x, Y2 = ropeBottom + 22,
            Stroke = "#111111",
            StrokeWidth = 2.5
        });

        // Hook forged curved profile
        drawing.Elements.Add(new DrawingArc
        {
            CenterX = x + 2,
            CenterY = ropeBottom + 30,
            Radius = 9,
            StartAngleDegrees = 40,
            EndAngleDegrees = 260,
            Fill = "none",
            Stroke = "#111111",
            StrokeWidth = 3
        });

        // Hook tip
        drawing.Elements.Add(new DrawingLine
        {
            X1 = x - 6, Y1 = ropeBottom + 26,
            X2 = x - 2, Y2 = ropeBottom + 22,
            Stroke = "#111111",
            StrokeWidth = 2.5
        });
    }

    protected static void AddGround(DrawingModel drawing, double left, double right, double ground)
    {
        // Ground surface line
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left - 25, Y1 = ground,
            X2 = right + 25, Y2 = ground,
            Stroke = "#333333",
            StrokeWidth = 2
        });

        // Hatching below ground
        for (var hx = left - 20; hx <= right + 20; hx += 16)
        {
            drawing.Elements.Add(new DrawingLine
            {
                X1 = hx, Y1 = ground,
                X2 = hx - 8, Y2 = ground + 8,
                Stroke = "#777777",
                StrokeWidth = 0.8
            });
        }
    }

    protected static void AddFoundation(DrawingModel drawing, double centerX, double groundY, double width = 80, double height = 18)
    {
        drawing.Elements.Add(new DrawingRectangle
        {
            X = centerX - width / 2,
            Y = groundY,
            Width = width,
            Height = height,
            Fill = "#bdc3c7",
            Stroke = "#2c3e50",
            StrokeWidth = 1.5
        });

        // Anchor bolts
        drawing.Elements.Add(new DrawingLine
        {
            X1 = centerX - width * 0.35, Y1 = groundY - 6,
            X2 = centerX - width * 0.35, Y2 = groundY + 12,
            Stroke = "#111111", StrokeWidth = 1.8
        });
        drawing.Elements.Add(new DrawingLine
        {
            X1 = centerX + width * 0.35, Y1 = groundY - 6,
            X2 = centerX + width * 0.35, Y2 = groundY + 12,
            Stroke = "#111111", StrokeWidth = 1.8
        });
    }

    protected static void AddCenterline(DrawingModel drawing, double x1, double y1, double x2, double y2)
    {
        drawing.Elements.Add(new DrawingCenterline
        {
            X1 = x1, Y1 = y1,
            X2 = x2, Y2 = y2,
            StrokeWidth = 0.8
        });
    }

    protected static void AddSectionCutMarker(DrawingModel drawing, double x1, double y1, double x2, double y2, string label = "A")
    {
        // Cut line
        drawing.Elements.Add(new DrawingLine
        {
            X1 = x1, Y1 = y1,
            X2 = x2, Y2 = y2,
            Stroke = "#c0392b",
            StrokeWidth = 1.5
        });

        // Arrows & labels at ends
        drawing.Elements.Add(new DrawingText
        {
            X = x1, Y = y1 - 4,
            Content = label,
            FontSize = 9, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#c0392b"
        });
        drawing.Elements.Add(new DrawingText
        {
            X = x2, Y = y2 + 12,
            Content = label,
            FontSize = 9, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#c0392b"
        });
    }

    protected static void AddDimensions(DrawingModel drawing, DesignResponse design, CraneCalculationResult calculation)
    {
        var elevation = drawing.Views[0];
        var plan = drawing.Views[1];
        var left = elevation.X + 80;
        var right = elevation.X + elevation.Width - 80;

        // 1. Primary Span Dimension (Elevation)
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left, Y1 = elevation.Y + 22,
            X2 = right, Y2 = elevation.Y + 22,
            Label = $"{design.SpanMm:N0} mm",
            Offset = 8,
            Orientation = "horizontal"
        });

        // 2. Lift Height Dimension (Elevation)
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = right + 25, Y1 = elevation.Y + 95,
            X2 = right + 25, Y2 = elevation.Y + elevation.Height - 32,
            Label = $"{design.LiftHeightMm:N0} mm",
            Offset = 10,
            Orientation = "vertical"
        });

        // 3. Building Height Dimension (Elevation, if > 0)
        if (design.BuildingHeightMm > 0)
        {
            drawing.Dimensions.Add(new DrawingDimension
            {
                X1 = left - 32, Y1 = elevation.Y + 45,
                X2 = left - 32, Y2 = elevation.Y + elevation.Height - 32,
                Label = $"{design.BuildingHeightMm:N0} mm",
                Offset = 10,
                Orientation = "vertical"
            });
        }

        // 4. Hoist Position Dimension (Elevation)
        if (design.HoistPositionMm > 0 && design.SpanMm > 0)
        {
            var hx = HoistX(left, right, calculation.HoistPositionRatio);
            drawing.Dimensions.Add(new DrawingDimension
            {
                X1 = left, Y1 = elevation.Y + 38,
                X2 = hx, Y2 = elevation.Y + 38,
                Label = $"Hoist {design.HoistPositionMm:N0} mm",
                Offset = 6,
                Orientation = "horizontal"
            });
        }

        // 5. Reference Clearances (Engineering honesty: marked as reference/estimate)
        // Left side clearance
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left - 25, Y1 = elevation.Y + 75,
            X2 = left, Y2 = elevation.Y + 75,
            Label = "Min. 235 [EST]",
            Offset = 5,
            IsReference = true,
            Orientation = "horizontal"
        });

        // Overhead clearance
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left + 40, Y1 = elevation.Y + 45,
            X2 = left + 40, Y2 = elevation.Y + 68,
            Label = "OVERHEAD 200 [EST]",
            Offset = 5,
            IsReference = true,
            Orientation = "vertical"
        });

        // Hook approach clearances
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left, Y1 = elevation.Y + elevation.Height - 50,
            X2 = left + 55, Y2 = elevation.Y + elevation.Height - 50,
            Label = "Min. 920 [EST]",
            Offset = 5,
            IsReference = true,
            Orientation = "horizontal"
        });

        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = right - 65, Y1 = elevation.Y + elevation.Height - 50,
            X2 = right, Y2 = elevation.Y + elevation.Height - 50,
            Label = "Min. 1150 [EST]",
            Offset = 5,
            IsReference = true,
            Orientation = "horizontal"
        });

        // 6. Plan View Runway Width Dimension
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = plan.X + 45, Y1 = plan.Y + plan.Height - 18,
            X2 = plan.X + plan.Width - 45, Y2 = plan.Y + plan.Height - 18,
            Label = $"{design.RunwayWidthMm:N0} mm",
            Offset = 6,
            Orientation = "horizontal"
        });
    }
}
