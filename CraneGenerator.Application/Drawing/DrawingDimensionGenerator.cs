using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing;

public static class DrawingDimensionGenerator
{
    public static DrawingDimension CreateHorizontal(
        double x1, double y1, double x2, double y2,
        string label, double offset = 15, bool isReference = false)
    {
        return new DrawingDimension
        {
            X1 = Math.Min(x1, x2),
            Y1 = y1,
            X2 = Math.Max(x1, x2),
            Y2 = y2,
            Label = label,
            Offset = offset,
            IsReference = isReference,
            Orientation = "horizontal"
        };
    }

    public static DrawingDimension CreateVertical(
        double x1, double y1, double x2, double y2,
        string label, double offset = 15, bool isReference = false)
    {
        return new DrawingDimension
        {
            X1 = x1,
            Y1 = Math.Min(y1, y2),
            X2 = x2,
            Y2 = Math.Max(y1, y2),
            Label = label,
            Offset = offset,
            IsReference = isReference,
            Orientation = "vertical"
        };
    }

    public static DrawingDimension CreateClearance(
        double x1, double y1, double x2, double y2,
        string label, double offset = 12)
    {
        return new DrawingDimension
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Label = label,
            Offset = offset,
            IsReference = true,
            Orientation = Math.Abs(x2 - x1) > Math.Abs(y2 - y1) ? "horizontal" : "vertical"
        };
    }

    public static DrawingDimension CreateDiameter(
        double cx, double cy, double radius,
        string label, double offset = 10)
    {
        return new DrawingDimension
        {
            X1 = cx - radius,
            Y1 = cy,
            X2 = cx + radius,
            Y2 = cy,
            Label = label,
            Offset = offset,
            IsReference = true,
            Orientation = "horizontal"
        };
    }
}
