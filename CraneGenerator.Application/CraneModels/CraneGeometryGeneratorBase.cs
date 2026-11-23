using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.CraneModels;

public abstract class CraneGeometryGeneratorBase : ICraneGeometryGenerator
{
    protected const string GirderFill = "#f4c542";
    protected const string SteelFill = "#eeeeee";
    protected const string HoistFill = "#4f81bd";

    public DrawingModel Generate(DesignResponse design)
    {
        var drawing = CreateDrawing(design);
        CreateElevation(drawing, design);
        CreatePlan(drawing, design);
        CreateSection(drawing, design);
        AddDimensions(drawing, design);
        return drawing;
    }

    protected abstract void CreateElevation(DrawingModel drawing, DesignResponse design);
    protected abstract void CreatePlan(DrawingModel drawing, DesignResponse design);
    protected abstract void CreateSection(DrawingModel drawing, DesignResponse design);

    protected static DrawingModel CreateDrawing(DesignResponse design)
    {
        var drawing = new DrawingModel
        {
            DrawingNumber = $"CRN-{design.Id:D3}",
            Title = "PARAMETRIC OVERHEAD CRANE DRAWING",
            Scale = "NTS"
        };

        drawing.Views.Add(new DrawingView { Name = "MAIN ELEVATION", X = 50, Y = 50, Width = 700, Height = 300 });
        drawing.Views.Add(new DrawingView { Name = "PLAN VIEW", X = 50, Y = 400, Width = 700, Height = 220 });
        drawing.Views.Add(new DrawingView { Name = "SECTION A-A", X = 800, Y = 400, Width = 300, Height = 220 });
        drawing.TechnicalData = new DrawingTechnicalData
        {
            CapacityTonnes = design.CapacityTonnes,
            SpanMm = design.SpanMm,
            LiftHeightMm = design.LiftHeightMm,
            RunwayWidthMm = design.RunwayWidthMm,
            HoistPositionMm = design.HoistPositionMm,
            BuildingHeightMm = design.BuildingHeightMm
        };
        return drawing;
    }

    protected static (double Left, double Right, double Ground, double Top) ElevationBounds(DrawingView view, DesignResponse design)
    {
        var left = view.X + 50;
        var right = view.X + view.Width - 50;
        var ground = view.Y + view.Height - 30;
        var verticalRange = Math.Clamp(design.BuildingHeightMm, 0, 100000) / 100000d * 170d;
        var top = ground - Math.Max(90, verticalRange);
        return (left, right, ground, top);
    }

    protected static double HoistX(double left, double right, double hoistPositionMm, double spanMm)
    {
        var ratio = spanMm <= 0 ? 0 : Math.Clamp(hoistPositionMm / spanMm, 0, 1);
        return left + ratio * (right - left);
    }

    protected static void AddHook(DrawingModel drawing, double x, double ropeTop, double ropeLength)
    {
        var ropeBottom = ropeTop + Math.Max(30, ropeLength);
        drawing.Elements.Add(new DrawingLine { X1 = x, Y1 = ropeTop, X2 = x, Y2 = ropeBottom, StrokeWidth = 2 });
        drawing.Elements.Add(new DrawingCircle { CenterX = x, CenterY = ropeBottom + 10, Radius = 10, StrokeWidth = 2 });
    }

    protected static void AddDimensions(DrawingModel drawing, DesignResponse design)
    {
        var elevation = drawing.Views[0];
        var plan = drawing.Views[1];
        var left = elevation.X + 75;
        var right = elevation.X + elevation.Width - 75;

        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left, Y1 = elevation.Y + 20, X2 = right, Y2 = elevation.Y + 20,
            Label = $"{design.SpanMm:N0} mm", Offset = 10
        });
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = right + 30, Y1 = elevation.Y + 110, X2 = right + 30,
            Y2 = elevation.Y + elevation.Height - 30,
            Label = $"{design.LiftHeightMm:N0} mm", Offset = 10
        });
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left - 30, Y1 = elevation.Y + 60, X2 = left - 30,
            Y2 = elevation.Y + elevation.Height - 30,
            Label = $"{design.BuildingHeightMm:N0} mm", Offset = 10
        });
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = plan.X + 50, Y1 = plan.Y + 190, X2 = plan.X + plan.Width - 50,
            Y2 = plan.Y + 190,
            Label = $"{design.RunwayWidthMm:N0} mm", Offset = 10
        });
        drawing.Dimensions.Add(new DrawingDimension
        {
            X1 = left, Y1 = elevation.Y + 40, X2 = HoistX(left, right, design.HoistPositionMm, design.SpanMm),
            Y2 = elevation.Y + 40,
            Label = $"Hoist {design.HoistPositionMm:N0} mm", Offset = 8
        });
    }

    protected static void AddGround(DrawingModel drawing, double left, double right, double ground)
    {
        drawing.Elements.Add(new DrawingLine
        {
            X1 = left - 20, Y1 = ground, X2 = right + 20, Y2 = ground, StrokeWidth = 2
        });
    }
}
