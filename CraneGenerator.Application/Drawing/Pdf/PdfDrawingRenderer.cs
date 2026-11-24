using CraneGenerator.Domain.Entities;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.IO;
using System.Linq;

namespace CraneGenerator.Application.Drawing.Pdf;

public sealed class PdfDrawingRenderer : IPdfDrawingRenderer
{
    private const double CanvasWidth = 1200;
    private const double CanvasHeight = 800;
    private const double Margin = 20;

    public byte[] Render(DrawingModel model)
    {
        ArgumentNullException.ThrowIfNull(model);
        PdfSharpLicense.SetAgplLicense();

        using var document = new PdfDocument();
        document.Info.Title = model.Title;
        document.Info.Subject = model.TechnicalData.DesignName;
        document.Info.Author = "CraneGenerator CAD System";

        var page = document.AddPage();
        page.Size = PageSize.A3;
        page.Orientation = PageOrientation.Landscape;

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

            // 1. Drawing Views (dashed view boundary boxes & view headers)
            foreach (var view in model.Views)
            {
                var rect = MapRect(view.X, view.Y, view.Width, view.Height);
                var dashedPen = new XPen(XColor.FromArgb(149, 165, 166), 0.7 * scale)
                {
                    DashStyle = XDashStyle.Dash
                };
                graphics.DrawRectangle(dashedPen, rect);

                var scaleText = !string.IsNullOrEmpty(view.Scale) && view.Scale != "NTS" ? $" ({view.Scale})" : "";
                DrawText(graphics, $"{view.Name}{scaleText}", Map(view.X + 6, view.Y - 5), 10.5 * scale, XStringFormats.BottomLeft, true, XColor.FromArgb(26, 82, 118));
            }

            // 2. Details (Detail Z, Runway Bay Diagram, Notes & End Stops)
            foreach (var detail in model.Details)
            {
                // Detail boundary box
                var dRect = MapRect(detail.X, detail.Y, detail.Width, detail.Height);
                graphics.DrawRectangle(new XPen(XColors.Black, 1 * scale), XBrushes.White, dRect);

                // Detail elements
                foreach (var elem in detail.Elements)
                {
                    RenderElement(graphics, elem, Map, MapRect, scale);
                }

                // Detail dimensions
                foreach (var dim in detail.Dimensions)
                {
                    RenderDimension(graphics, dim, Map, scale);
                }
            }

            // 3. Technical Specifications Table
            RenderTechnicalDataTable(graphics, model, Map, MapRect, scale);

            // 4. Main Model Elements
            foreach (var element in model.Elements)
            {
                RenderElement(graphics, element, Map, MapRect, scale);
            }

            // 5. Main Model Dimensions
            foreach (var dimension in model.Dimensions)
            {
                RenderDimension(graphics, dimension, Map, scale);
            }
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

            case DrawingRectangle rect:
                var r = mapRect(rect.X, rect.Y, rect.Width, rect.Height);
                if (TryParseFill(rect.Fill, out var rectBrush))
                    graphics.DrawRectangle(pen, rectBrush, r);
                else
                    graphics.DrawRectangle(pen, r);
                break;

            case DrawingCircle circle:
                var center = map(circle.CenterX, circle.CenterY);
                var dia = circle.Radius * 2 * scale;
                var cRect = new XRect(center.X - dia / 2, center.Y - dia / 2, dia, dia);
                if (TryParseFill(circle.Fill, out var circleBrush))
                    graphics.DrawEllipse(pen, circleBrush, cRect);
                else
                    graphics.DrawEllipse(pen, cRect);
                break;

            case DrawingPolyline polyline:
                if (polyline.Points.Count < 2)
                    break;
                var pts = polyline.Points.Select(p => map(p.X, p.Y)).ToArray();
                graphics.DrawLines(pen, pts);
                if (polyline.IsClosed && polyline.Points.Count >= 3)
                    graphics.DrawLine(pen, pts[^1], pts[0]);
                break;

            case DrawingArc arc:
                RenderArc(graphics, arc, map, scale);
                break;

            case DrawingText text:
                RenderTextElement(graphics, text, map, scale);
                break;

            case DrawingCenterline centerline:
                var dashedPen = new XPen(ParseColor(centerline.Stroke), Math.Max(0.5, centerline.StrokeWidth * scale))
                {
                    DashStyle = XDashStyle.DashDot
                };
                graphics.DrawLine(dashedPen, map(centerline.X1, centerline.Y1), map(centerline.X2, centerline.Y2));
                break;
        }
    }

    private static void RenderArc(
        XGraphics graphics,
        DrawingArc arc,
        Func<double, double, XPoint> map,
        double scale)
    {
        var center = map(arc.CenterX, arc.CenterY);
        var dia = arc.Radius * 2 * scale;
        var arcRect = new XRect(center.X - dia / 2, center.Y - dia / 2, dia, dia);
        var pen = new XPen(ParseColor(arc.Stroke), Math.Max(0.5, arc.StrokeWidth * scale))
        {
            DashStyle = XDashStyle.Dash
        };

        var sweep = arc.EndAngleDegrees - arc.StartAngleDegrees;
        if (sweep < 0) sweep += 360;

        if (Math.Abs(sweep - 360) < 0.1 || sweep <= 0)
        {
            graphics.DrawEllipse(pen, arcRect);
            return;
        }

        graphics.DrawArc(pen, arcRect, arc.StartAngleDegrees, sweep);
    }

    private static void RenderTextElement(
        XGraphics graphics,
        DrawingText text,
        Func<double, double, XPoint> map,
        double scale)
    {
        var point = map(text.X, text.Y);
        var fontSize = Math.Max(5.5, text.FontSize * scale);
        var bold = text.FontWeight == "bold";

        var format = text.TextAnchor switch
        {
            "middle" => XStringFormats.TopCenter,
            "end" => XStringFormats.TopRight,
            _ => XStringFormats.TopLeft
        };

        DrawText(graphics, text.Content, point, fontSize, format, bold, ParseColor(text.Fill));
    }

    private static void RenderDimension(
        XGraphics graphics,
        DrawingDimension dim,
        Func<double, double, XPoint> map,
        double scale)
    {
        var start = map(dim.X1, dim.Y1);
        var end = map(dim.X2, dim.Y2);
        var strokeColor = dim.IsReference ? XColor.FromArgb(80, 80, 80) : XColors.Black;
        var pen = new XPen(strokeColor, Math.Max(0.45, 0.85 * scale));

        // Dimension leader line
        graphics.DrawLine(pen, start, end);

        // Extension lines
        var isHoriz = Math.Abs(dim.Y2 - dim.Y1) < Math.Abs(dim.X2 - dim.X1);
        var ext = Math.Max(6, (dim.Offset + 4) * scale);

        if (isHoriz)
        {
            graphics.DrawLine(pen, new XPoint(start.X, start.Y - ext), new XPoint(start.X, start.Y + ext));
            graphics.DrawLine(pen, new XPoint(end.X, end.Y - ext), new XPoint(end.X, end.Y + ext));
        }
        else
        {
            graphics.DrawLine(pen, new XPoint(start.X - ext, start.Y), new XPoint(start.X + ext, start.Y));
            graphics.DrawLine(pen, new XPoint(end.X - ext, end.Y), new XPoint(end.X + ext, end.Y));
        }

        // Arrowheads
        DrawArrow(graphics, start, end, scale);
        DrawArrow(graphics, end, start, scale);

        // Label
        var midX = (start.X + end.X) / 2;
        var midY = (start.Y + end.Y) / 2;

        double lblX = midX;
        double lblY = isHoriz ? midY - dim.Offset * scale : midY;
        var fmt = isHoriz ? XStringFormats.BottomCenter : XStringFormats.CenterLeft;
        if (!isHoriz)
            lblX = midX + (dim.Offset + 2) * scale;

        DrawText(graphics, dim.Label, new XPoint(lblX, lblY), Math.Max(5.5, 8.5 * scale), fmt, true, strokeColor);
    }

    private static void DrawArrow(XGraphics graphics, XPoint from, XPoint target, double scale)
    {
        var dx = target.X - from.X;
        var dy = target.Y - from.Y;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len <= 1e-4) return;

        var ux = dx / len;
        var uy = dy / len;
        var arrowLen = 7 * scale;
        var arrowW = 2.8 * scale;

        var basePt = new XPoint(from.X + ux * arrowLen, from.Y + uy * arrowLen);
        var perp = new XPoint(-uy, ux);
        var points = new[]
        {
            from,
            new XPoint(basePt.X + perp.X * arrowW, basePt.Y + perp.Y * arrowW),
            new XPoint(basePt.X - perp.X * arrowW, basePt.Y - perp.Y * arrowW)
        };
        graphics.DrawPolygon(XBrushes.Black, points, XFillMode.Winding);
    }

    private static void RenderTechnicalDataTable(
        XGraphics graphics,
        DrawingModel model,
        Func<double, double, XPoint> map,
        Func<double, double, double, double, XRect> mapRect,
        double scale)
    {
        const double x = 810;
        const double y = 58;
        const double w = 355;
        const double h = 498;

        // Table outer frame
        graphics.DrawRectangle(new XPen(XColors.Black, 1.2 * scale), XBrushes.White, mapRect(x, y, w, h));

        // Header bar
        graphics.DrawRectangle(XPens.Black, new XSolidBrush(XColor.FromArgb(27, 79, 114)), mapRect(x, y, w, 22));
        DrawText(graphics, "TECHNICAL SPECIFICATIONS", map(x + w / 2, y + 15), 10 * scale, XStringFormats.Center, true, XColors.White);

        // Subheader status bar
        graphics.DrawRectangle(XPens.Gray, new XSolidBrush(XColor.FromArgb(234, 242, 248)), mapRect(x, y + 22, w, 14));
        DrawText(graphics, "STATUS: [USER INPUT] [DERIVED] [ESTIMATE] [PROTOTYPE] [REF]", map(x + 8, y + 32), 6.5 * scale, XStringFormats.TopLeft, true, XColor.FromArgb(21, 67, 96));

        var specs = model.TechnicalData.Specifications;
        const double rowH = 20;
        var curY = y + 36;

        for (var i = 0; i < specs.Count && curY + rowH < y + h - 30; i++)
        {
            var spec = specs[i];
            var rowColor = i % 2 == 0 ? XColors.White : XColor.FromArgb(248, 249, 249);
            graphics.DrawRectangle(new XPen(XColor.FromArgb(229, 231, 233), 0.5 * scale), new XSolidBrush(rowColor), mapRect(x, curY, w, rowH));

            // Label
            DrawText(graphics, spec.Label, map(x + 10, curY + 14), 7.8 * scale, XStringFormats.TopLeft, false, XColor.FromArgb(44, 62, 80));

            // Value
            DrawText(graphics, spec.Value, map(x + w - 82, curY + 14), 7.8 * scale, XStringFormats.TopRight, true, XColors.Black);

            // Source tag
            var tagColor = spec.Source switch
            {
                SpecificationSource.UserInput => XColor.FromArgb(17, 122, 101),
                SpecificationSource.Derived => XColor.FromArgb(31, 97, 141),
                SpecificationSource.Estimate => XColor.FromArgb(211, 84, 0),
                SpecificationSource.PrototypeRule => XColor.FromArgb(142, 68, 173),
                _ => XColor.FromArgb(85, 85, 85)
            };
            DrawText(graphics, spec.SourceTag, map(x + w - 8, curY + 14), 6.8 * scale, XStringFormats.TopRight, true, tagColor);

            curY += rowH;
        }

        // Table bottom warning note
        graphics.DrawLine(new XPen(XColors.Black, 0.8 * scale), map(x, y + h - 30), map(x + w, y + h - 30));
        DrawText(graphics, "* Values marked [ESTIMATE] or [PROTOTYPE] are reference defaults", map(x + 8, y + h - 18), 6.5 * scale, XStringFormats.TopLeft, false, XColor.FromArgb(183, 28, 28));
        DrawText(graphics, "  for conceptual layout only and NOT certified engineering calculations.", map(x + 8, y + h - 8), 6.5 * scale, XStringFormats.TopLeft, false, XColor.FromArgb(183, 28, 28));
    }

    private static void DrawText(
        XGraphics graphics,
        string text,
        XPoint point,
        double fontSize,
        XStringFormat format,
        bool bold = false,
        XColor? color = null)
    {
        var fontStyle = bold ? XFontStyleEx.Bold : XFontStyleEx.Regular;
        var font = new XFont("Liberation Sans", Math.Max(4.5, fontSize), fontStyle);
        var brush = new XSolidBrush(color ?? XColors.Black);
        graphics.DrawString(text, font, brush, point, format);
    }

    private static XColor ParseColor(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor) || hexColor == "#000000" || hexColor == "black")
            return XColors.Black;
        if (hexColor == "#ffffff" || hexColor == "white")
            return XColors.White;

        if (hexColor.StartsWith("#"))
            hexColor = hexColor[1..];

        if (hexColor.Length == 6 && int.TryParse(hexColor, System.Globalization.NumberStyles.HexNumber, null, out var rgb))
        {
            var r = (byte)((rgb >> 16) & 0xFF);
            var g = (byte)((rgb >> 8) & 0xFF);
            var b = (byte)(rgb & 0xFF);
            return XColor.FromArgb(r, g, b);
        }
        return XColors.Black;
    }

    private static bool TryParseFill(string fill, out XBrush brush)
    {
        brush = default!;
        if (string.IsNullOrEmpty(fill) || fill == "none")
            return false;

        brush = new XSolidBrush(ParseColor(fill));
        return true;
    }
}
