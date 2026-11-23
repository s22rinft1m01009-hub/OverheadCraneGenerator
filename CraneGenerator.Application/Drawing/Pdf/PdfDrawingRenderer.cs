using CraneGenerator.Domain.Entities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace CraneGenerator.Application.Drawing.Pdf;

public sealed class PdfDrawingRenderer : IPdfDrawingRenderer
{
    private const double CanvasWidth = 1200;
    private const double CanvasHeight = 800;
    private const double Margin = 24;

    public byte[] Render(DrawingModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        using var document = new PdfDocument();
        document.Info.Title = model.Title;
        document.Info.Subject = model.TechnicalData.DesignName;
        var page = document.AddPage();
        page.Size = PdfSharp.PageSize.A3;
        page.Orientation = PdfSharp.PageOrientation.Landscape;

        using (var graphics = XGraphics.FromPdfPage(page))
        {
            var scale = Math.Min(
                (page.Width.Point - Margin * 2) / CanvasWidth,
                (page.Height.Point - Margin * 2) / CanvasHeight);
            var offsetX = (page.Width.Point - CanvasWidth * scale) / 2;
            var offsetY = (page.Height.Point - CanvasHeight * scale) / 2;

            XPoint Map(double x, double y) => new(offsetX + x * scale, offsetY + y * scale);
            XRect MapRect(double x, double y, double width, double height)
            {
                var point = Map(x, y);
                return new XRect(point.X, point.Y, width * scale, height * scale);
            }

            var borderPen = new XPen(XColors.Black, 1.2);
            graphics.DrawRectangle(borderPen, MapRect(0, 0, CanvasWidth, CanvasHeight));
            DrawText(graphics, model.Title, Map(600, 25), 18 * scale, XStringFormats.TopCenter, true);

            foreach (var view in model.Views)
            {
                var rect = MapRect(view.X, view.Y, view.Width, view.Height);
                var dashedPen = new XPen(XColors.Gray, 0.6) { DashStyle = XDashStyle.Dash };
                graphics.DrawRectangle(dashedPen, rect);
                DrawText(graphics, view.Name, Map(view.X, view.Y - 4), 12 * scale, XStringFormats.BottomLeft, true);
            }

            foreach (var element in model.Elements)
                RenderElement(graphics, element, Map, MapRect, scale);

            foreach (var dimension in model.Dimensions)
                RenderDimension(graphics, dimension, Map, scale);

            RenderTechnicalData(graphics, model, Map, MapRect, scale);
            RenderTitleBlock(graphics, model, Map, MapRect, scale);
        }

        using var output = new MemoryStream();
        document.Save(output);
        return output.ToArray();
    }

    private static void RenderElement(
        XGraphics graphics,
        DrawingElement element,
        Func<double, double, XPoint> map,
        Func<double, double, double, double, XRect> mapRect,
        double scale)
    {
        var pen = new XPen(ParseColor(element.Stroke), Math.Max(0.5, element.StrokeWidth * scale));
        switch (element)
        {
            case DrawingLine line:
                graphics.DrawLine(pen, map(line.X1, line.Y1), map(line.X2, line.Y2));
                break;
            case DrawingRectangle rectangle:
                var rect = mapRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                if (TryParseFill(rectangle.Fill, out var rectangleBrush))
                    graphics.DrawRectangle(pen, rectangleBrush, rect);
                else
                    graphics.DrawRectangle(pen, rect);
                break;
            case DrawingCircle circle:
                var center = map(circle.CenterX, circle.CenterY);
                var diameter = circle.Radius * 2 * scale;
                var circleRect = new XRect(center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter);
                if (TryParseFill(circle.Fill, out var circleBrush))
                    graphics.DrawEllipse(pen, circleBrush, circleRect);
                else
                    graphics.DrawEllipse(pen, circleRect);
                break;
        }
    }

    private static void RenderDimension(
        XGraphics graphics,
        DrawingDimension dimension,
        Func<double, double, XPoint> map,
        double scale)
    {
        var start = map(dimension.X1, dimension.Y1);
        var end = map(dimension.X2, dimension.Y2);
        var pen = new XPen(XColors.Black, Math.Max(0.45, scale));
        graphics.DrawLine(pen, start, end);

        var extension = 15 * scale;
        graphics.DrawLine(pen, new XPoint(start.X, start.Y - extension), new XPoint(start.X, start.Y + extension));
        graphics.DrawLine(pen, new XPoint(end.X, end.Y - extension), new XPoint(end.X, end.Y + extension));
        DrawArrow(graphics, start, end, scale);
        DrawArrow(graphics, end, start, scale);

        var labelX = (start.X + end.X) / 2;
        var labelY = (start.Y + end.Y) / 2 - dimension.Offset * scale;
        DrawText(graphics, dimension.Label, new XPoint(labelX, labelY), Math.Max(6, 11 * scale), XStringFormats.BottomCenter);
    }

    private static void DrawArrow(XGraphics graphics, XPoint point, XPoint target, double scale)
    {
        var dx = target.X - point.X;
        var dy = target.Y - point.Y;
        var length = Math.Sqrt(dx * dx + dy * dy);
        if (length <= double.Epsilon)
            return;

        var ux = dx / length;
        var uy = dy / length;
        var arrowLength = 8 * scale;
        var arrowWidth = 4 * scale;
        var basePoint = new XPoint(point.X + ux * arrowLength, point.Y + uy * arrowLength);
        var perpendicular = new XPoint(-uy, ux);
        var points = new[]
        {
            point,
            new XPoint(basePoint.X + perpendicular.X * arrowWidth, basePoint.Y + perpendicular.Y * arrowWidth),
            new XPoint(basePoint.X - perpendicular.X * arrowWidth, basePoint.Y - perpendicular.Y * arrowWidth)
        };
        graphics.DrawPolygon(XBrushes.Black, points, XFillMode.Winding);
    }

    private static void RenderTechnicalData(
        XGraphics graphics,
        DrawingModel model,
        Func<double, double, XPoint> map,
        Func<double, double, double, double, XRect> mapRect,
        double scale)
    {
        const double x = 800;
        const double y = 50;
        const double width = 300;
        const double height = 320;
        graphics.DrawRectangle(XPens.Black, mapRect(x, y, width, height));
        var row = y + 24;
        DrawText(graphics, "TECHNICAL DATA", map(x + 10, row), 14 * scale, XStringFormats.TopLeft, true);
        row += 30;
        AddRow("Design", model.TechnicalData.DesignName);
        AddRow("Crane Type", model.TechnicalData.CraneType);
        AddRow("Capacity", $"{model.TechnicalData.CapacityTonnes:0.##} tonnes");
        AddRow("Span", $"{model.TechnicalData.SpanMm:0} mm");
        AddRow("Lift Height", $"{model.TechnicalData.LiftHeightMm:0} mm");
        AddRow("Runway Width", $"{model.TechnicalData.RunwayWidthMm:0} mm");
        AddRow("Hoist Position", $"{model.TechnicalData.HoistPositionMm:0} mm");
        AddRow("Building Height", $"{model.TechnicalData.BuildingHeightMm:0} mm");

        void AddRow(string label, string value)
        {
            DrawText(graphics, label, map(x + 10, row), 9 * scale, XStringFormats.TopLeft);
            DrawText(graphics, value, map(x + width - 10, row), 9 * scale, XStringFormats.TopRight);
            row += 25;
        }
    }

    private static void RenderTitleBlock(
        XGraphics graphics,
        DrawingModel model,
        Func<double, double, XPoint> map,
        Func<double, double, double, double, XRect> mapRect,
        double scale)
    {
        const double x = 50;
        const double y = 650;
        const double width = 1050;
        const double height = 100;
        graphics.DrawRectangle(new XPen(XColors.Black, 1.2), mapRect(x, y, width, height));
        DrawText(graphics, "PARAMETRIC OVERHEAD CRANE", map(x + 20, y + 18), 14 * scale, XStringFormats.TopLeft, true);
        DrawText(graphics, model.TechnicalData.Note, map(x + 20, y + 40), 12 * scale, XStringFormats.TopLeft, true);
        DrawText(graphics, $"Drawing No: {model.DrawingNumber}", map(x + 20, y + 65), 10 * scale, XStringFormats.TopLeft);
        DrawText(graphics, $"Scale: {model.Scale}", map(x + 400, y + 65), 10 * scale, XStringFormats.TopLeft);
        DrawText(graphics, $"Revision: {model.TechnicalData.Revision}", map(x + 700, y + 65), 10 * scale, XStringFormats.TopLeft);
        DrawText(graphics, DateTime.UtcNow.ToString("yyyy-MM-dd"), map(x + width - 15, y + 65), 10 * scale, XStringFormats.TopRight);
    }

    private static void DrawText(XGraphics graphics, string? text, XPoint point, double size, XStringFormat format, bool bold = false)
    {
        var font = new XFont("Arial", Math.Max(5, size), bold ? XFontStyleEx.Bold : XFontStyleEx.Regular);
        graphics.DrawString(text ?? string.Empty, font, XBrushes.Black, point, format);
    }

    private static XColor ParseColor(string color)
    {
        try
        {
            return XColor.FromName(color);
        }
        catch
        {
            return XColors.Black;
        }
    }

    private static bool TryParseFill(string fill, out XBrush brush)
    {
        if (string.IsNullOrWhiteSpace(fill) || string.Equals(fill, "none", StringComparison.OrdinalIgnoreCase))
        {
            brush = XBrushes.Transparent;
            return false;
        }

        try
        {
            brush = new XSolidBrush(XColor.FromName(fill));
            return true;
        }
        catch
        {
            brush = XBrushes.Transparent;
            return false;
        }
    }
}
