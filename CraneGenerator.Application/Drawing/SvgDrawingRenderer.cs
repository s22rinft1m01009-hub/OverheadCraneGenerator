using CraneGenerator.Domain.Entities;
using System;
using System.Linq;
using System.Security;
using System.Text;

namespace CraneGenerator.Application.Drawing;

public class SvgDrawingRenderer
{
    private const double CanvasWidth = 1200;
    private const double CanvasHeight = 800;

    public string Render(DrawingModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var svg = new StringBuilder();

        svg.AppendLine($"""
        <svg xmlns="http://www.w3.org/2000/svg"
             width="{CanvasWidth}"
             height="{CanvasHeight}"
             viewBox="0 0 {CanvasWidth} {CanvasHeight}"
             font-family="Arial, 'Liberation Sans', sans-serif">

            <!-- Sheet Background -->
            <rect x="0" y="0" width="{CanvasWidth}" height="{CanvasHeight}" fill="#ffffff" />
        """);

        // 1. Drawing Views (Dashed borders & view labels)
        foreach (var view in model.Views)
        {
            AddViewBorder(svg, view);
            var scaleText = !string.IsNullOrEmpty(view.Scale) && view.Scale != "NTS" ? $" ({view.Scale})" : "";
            AddText(svg, view.X + 6, view.Y - 5, $"{view.Name}{scaleText}", 11, "start", true, "#1a5276");
        }

        // 2. Details (Detail Z, Runway Bay Diagram, Notes & End Stops)
        foreach (var detail in model.Details)
        {
            // Detail box frame
            svg.AppendLine($"""
            <rect x="{detail.X}" y="{detail.Y}" width="{detail.Width}" height="{detail.Height}"
                  fill="#ffffff" stroke="#333333" stroke-width="1" />
            """);

            // Detail elements
            foreach (var elem in detail.Elements)
            {
                RenderElement(svg, elem);
            }

            // Detail dimensions
            foreach (var dim in detail.Dimensions)
            {
                RenderDimension(svg, dim);
            }
        }

        // 3. Technical Specifications Table
        RenderTechnicalDataTable(svg, model);

        // 4. Main Model Elements
        foreach (var element in model.Elements)
        {
            RenderElement(svg, element);
        }

        // 5. Main Model Dimensions
        foreach (var dimension in model.Dimensions)
        {
            RenderDimension(svg, dimension);
        }

        svg.AppendLine("</svg>");
        return svg.ToString();
    }

    private static void RenderElement(StringBuilder svg, DrawingElement element)
    {
        switch (element)
        {
            case DrawingLine line:
                svg.AppendLine($"""
                <line x1="{line.X1:0.##}" y1="{line.Y1:0.##}" x2="{line.X2:0.##}" y2="{line.Y2:0.##}"
                      stroke="{line.Stroke}" stroke-width="{line.StrokeWidth:0.##}" />
                """);
                break;

            case DrawingRectangle rect:
                svg.AppendLine($"""
                <rect x="{rect.X:0.##}" y="{rect.Y:0.##}" width="{rect.Width:0.##}" height="{rect.Height:0.##}"
                      fill="{rect.Fill}" stroke="{rect.Stroke}" stroke-width="{rect.StrokeWidth:0.##}" />
                """);
                break;

            case DrawingCircle circle:
                svg.AppendLine($"""
                <circle cx="{circle.CenterX:0.##}" cy="{circle.CenterY:0.##}" r="{circle.Radius:0.##}"
                        fill="{circle.Fill}" stroke="{circle.Stroke}" stroke-width="{circle.StrokeWidth:0.##}" />
                """);
                break;

            case DrawingPolyline polyline:
                if (polyline.Points.Count < 2)
                    break;
                var pts = string.Join(" ", polyline.Points.Select(p => $"{p.X:0.##},{p.Y:0.##}"));
                var closed = polyline.IsClosed && polyline.Points.Count >= 3 ? " Z" : "";
                svg.AppendLine($"""
                <polyline points="{pts}{closed}"
                          fill="{polyline.Fill}" stroke="{polyline.Stroke}" stroke-width="{polyline.StrokeWidth:0.##}" />
                """);
                break;

            case DrawingArc arc:
                RenderArc(svg, arc);
                break;

            case DrawingText text:
                var escaped = SecurityElement.Escape(text.Content);
                var weight = text.FontWeight == "bold" ? "bold" : "normal";
                svg.AppendLine($"""
                <text x="{text.X:0.##}" y="{text.Y:0.##}"
                      font-size="{text.FontSize:0.##}" font-weight="{weight}"
                      text-anchor="{text.TextAnchor}" fill="{text.Fill}">
                    {escaped}
                </text>
                """);
                break;

            case DrawingCenterline centerline:
                svg.AppendLine($"""
                <line x1="{centerline.X1:0.##}" y1="{centerline.Y1:0.##}" x2="{centerline.X2:0.##}" y2="{centerline.Y2:0.##}"
                      stroke="{centerline.Stroke}" stroke-width="{centerline.StrokeWidth:0.##}" stroke-dasharray="8,4,2,4" />
                """);
                break;
        }
    }

    private static void RenderArc(StringBuilder svg, DrawingArc arc)
    {
        var startRad = arc.StartAngleDegrees * Math.PI / 180.0;
        var endRad = arc.EndAngleDegrees * Math.PI / 180.0;

        var x1 = arc.CenterX + arc.Radius * Math.Cos(startRad);
        var y1 = arc.CenterY + arc.Radius * Math.Sin(startRad);
        var x2 = arc.CenterX + arc.Radius * Math.Cos(endRad);
        var y2 = arc.CenterY + arc.Radius * Math.Sin(endRad);

        var sweepAngle = arc.EndAngleDegrees - arc.StartAngleDegrees;
        if (sweepAngle < 0) sweepAngle += 360;

        var largeArc = sweepAngle > 180 ? 1 : 0;
        const int sweepFlag = 1;

        if (Math.Abs(sweepAngle - 360) < 0.1 || sweepAngle <= 0)
        {
            // Full circle arc
            svg.AppendLine($"""
            <circle cx="{arc.CenterX:0.##}" cy="{arc.CenterY:0.##}" r="{arc.Radius:0.##}"
                    fill="{arc.Fill}" stroke="{arc.Stroke}" stroke-width="{arc.StrokeWidth:0.##}" stroke-dasharray="4,4" />
            """);
            return;
        }

        var pathData = $"M {x1:0.##} {y1:0.##} A {arc.Radius:0.##} {arc.Radius:0.##} 0 {largeArc} {sweepFlag} {x2:0.##} {y2:0.##}";

        svg.AppendLine($"""
        <path d="{pathData}" fill="{arc.Fill}" stroke="{arc.Stroke}" stroke-width="{arc.StrokeWidth:0.##}" stroke-dasharray="4,4" />
        """);
    }

    private static void RenderDimension(StringBuilder svg, DrawingDimension dim)
    {
        var x1 = dim.X1;
        var y1 = dim.Y1;
        var x2 = dim.X2;
        var y2 = dim.Y2;

        var isHoriz = Math.Abs(y2 - y1) < Math.Abs(x2 - x1);
        var stroke = dim.IsReference ? "#555555" : "#000000";

        // Dimension leader line
        svg.AppendLine($"""
        <line x1="{x1:0.##}" y1="{y1:0.##}" x2="{x2:0.##}" y2="{y2:0.##}"
              stroke="{stroke}" stroke-width="0.9" />
        """);

        // Extension lines
        if (isHoriz)
        {
            var ext = Math.Max(10, dim.Offset + 5);
            svg.AppendLine($"""
            <line x1="{x1:0.##}" y1="{y1 - ext:0.##}" x2="{x1:0.##}" y2="{y1 + ext:0.##}" stroke="{stroke}" stroke-width="0.7" />
            <line x2="{x2:0.##}" y1="{y2 - ext:0.##}" x2="{x2:0.##}" y2="{y2 + ext:0.##}" stroke="{stroke}" stroke-width="0.7" />
            """);
        }
        else
        {
            var ext = Math.Max(10, dim.Offset + 5);
            svg.AppendLine($"""
            <line x1="{x1 - ext:0.##}" y1="{y1:0.##}" x2="{x1 + ext:0.##}" y2="{y1:0.##}" stroke="{stroke}" stroke-width="0.7" />
            <line x1="{x2 - ext:0.##}" y1="{y2:0.##}" x2="{x2 + ext:0.##}" y2="{y2:0.##}" stroke="{stroke}" stroke-width="0.7" />
            """);
        }

        // Arrowheads
        AddArrow(svg, x1, y1, x2, y2);
        AddArrow(svg, x2, y2, x1, y1);

        // Label
        var midX = (x1 + x2) / 2;
        var midY = (y1 + y2) / 2;

        double lblX = midX;
        double lblY = isHoriz ? midY - dim.Offset : midY - 2;
        if (!isHoriz)
            lblX = midX + dim.Offset;

        AddText(svg, lblX, lblY, dim.Label, 8.5, isHoriz ? "middle" : "start", true, stroke);
    }

    private static void AddArrow(StringBuilder svg, double fromX, double fromY, double toX, double toY)
    {
        var dx = toX - fromX;
        var dy = toY - fromY;
        var len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1e-4) return;

        var ux = dx / len;
        var uy = dy / len;
        const double arrowLen = 7;
        const double arrowW = 2.8;

        var bx = fromX + ux * arrowLen;
        var by = fromY + uy * arrowLen;

        var p1X = bx - uy * arrowW;
        var p1Y = by + ux * arrowW;
        var p2X = bx + uy * arrowW;
        var p2Y = by - ux * arrowW;

        svg.AppendLine($"""
        <polygon points="{fromX:0.##},{fromY:0.##} {p1X:0.##},{p1Y:0.##} {p2X:0.##},{p2Y:0.##}" fill="#000000" />
        """);
    }

    private static void RenderTechnicalDataTable(StringBuilder svg, DrawingModel model)
    {
        const double tableX = 810;
        const double tableY = 58;
        const double tableW = 355;
        const double tableH = 498;

        // Table outer frame
        svg.AppendLine($"""
        <rect x="{tableX}" y="{tableY}" width="{tableW}" height="{tableH}"
              fill="#ffffff" stroke="#000000" stroke-width="1.5" />
        """);

        // Header bar
        svg.AppendLine($"""
        <rect x="{tableX}" y="{tableY}" width="{tableW}" height="22"
              fill="#1b4f72" stroke="#000000" stroke-width="1" />
        """);
        AddText(svg, tableX + tableW / 2, tableY + 15, "TECHNICAL SPECIFICATIONS", 10, "middle", true, "#ffffff");

        // Source classification legend bar
        svg.AppendLine($"""
        <rect x="{tableX}" y="{tableY + 22}" width="{tableW}" height="14"
              fill="#eaf2f8" stroke="#000000" stroke-width="0.8" />
        """);
        AddText(svg, tableX + 8, tableY + 32, "STATUS: [USER INPUT] [DERIVED] [ESTIMATE] [PROTOTYPE] [REF]", 6.5, "start", true, "#154360");

        var specs = model.TechnicalData.Specifications;
        const double rowH = 20;
        var curY = tableY + 36;

        for (var i = 0; i < specs.Count && curY + rowH < tableY + tableH - 30; i++)
        {
            var spec = specs[i];
            var rowBg = i % 2 == 0 ? "#ffffff" : "#f8f9f9";

            svg.AppendLine($"""
            <rect x="{tableX}" y="{curY}" width="{tableW}" height="{rowH}" fill="{rowBg}" stroke="#e5e7e9" stroke-width="0.6" />
            """);

            // Label
            AddText(svg, tableX + 10, curY + 14, spec.Label, 8, "start", false, "#2c3e50");

            // Value
            AddText(svg, tableX + tableW - 82, curY + 14, spec.Value, 8, "end", true, "#111111");

            // Source Tag
            var tagColor = spec.Source switch
            {
                SpecificationSource.UserInput => "#117a65",
                SpecificationSource.Derived => "#1f618d",
                SpecificationSource.Estimate => "#d35400",
                SpecificationSource.PrototypeRule => "#8e44ad",
                _ => "#555555"
            };
            AddText(svg, tableX + tableW - 8, curY + 14, spec.SourceTag, 6.8, "end", true, tagColor);

            curY += rowH;
        }

        // Table bottom warning note (Engineering honesty)
        svg.AppendLine($"""
        <line x1="{tableX}" y1="{tableY + tableH - 30}" x2="{tableX + tableW}" y2="{tableY + tableH - 30}" stroke="#000000" stroke-width="0.8" />
        """);
        AddText(svg, tableX + 8, tableY + tableH - 18, "* Values marked [ESTIMATE] or [PROTOTYPE] are reference defaults", 6.5, "start", false, "#b71c1c");
        AddText(svg, tableX + 8, tableY + tableH - 8, "  for conceptual layout only and NOT certified engineering calculations.", 6.5, "start", false, "#b71c1c");
    }

    private static void AddViewBorder(StringBuilder svg, DrawingView view)
    {
        svg.AppendLine($"""
        <rect x="{view.X}" y="{view.Y}" width="{view.Width}" height="{view.Height}"
              fill="none" stroke="#95a5a6" stroke-width="0.9" stroke-dasharray="4,4" />
        """);
    }

    private static void AddText(StringBuilder svg, double x, double y, string text, double fontSize, string anchor = "start", bool bold = false, string fill = "#000000")
    {
        var escaped = SecurityElement.Escape(text);
        var weight = bold ? "bold" : "normal";

        svg.AppendLine($"""
        <text x="{x:0.##}" y="{y:0.##}"
              font-size="{fontSize:0.##}" font-weight="{weight}"
              text-anchor="{anchor}" fill="{fill}">
            {escaped}
        </text>
        """);
    }
}
