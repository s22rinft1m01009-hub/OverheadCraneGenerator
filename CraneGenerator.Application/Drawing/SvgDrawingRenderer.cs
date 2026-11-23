using CraneGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace CraneGenerator.Application.Drawing
{
    public class SvgDrawingRenderer
    {
        public string Render(DrawingModel model)
        {
            const double canvasWidth = 1200;
            const double canvasHeight = 800;

            var svg = new StringBuilder();

            svg.AppendLine($"""
        <svg xmlns="http://www.w3.org/2000/svg"
             width="{canvasWidth}"
             height="{canvasHeight}"
             viewBox="0 0 {canvasWidth} {canvasHeight}">

            <rect x="0" y="0"
                  width="{canvasWidth}"
                  height="{canvasHeight}"
                  fill="white"
                  stroke="black"
                  stroke-width="2"/>

        """);

            // Title
            AddText(
                svg,
                600,
                25,
                model.Title,
                18,
                "middle",
                true);

            // Render drawing views
            foreach (var view in model.Views)
            {
                AddViewBorder(svg, view);
                AddText(
                    svg,
                    view.X,
                    view.Y - 10,
                    view.Name,
                    12,
                    "start",
                    true);
            }

            // Render elements
            foreach (var element in model.Elements)
            {
                RenderElement(svg, element);
            }

            // Render dimensions
            foreach (var dimension in model.Dimensions)
            {
                RenderDimension(svg, dimension);
            }

            // Technical data
            RenderTechnicalData(svg, model);

            // Title block
            RenderTitleBlock(svg, model);

            svg.AppendLine("</svg>");

            return svg.ToString();
        }

        private static void RenderElement(
            StringBuilder svg,
            DrawingElement element)
        {
            switch (element)
            {
                case DrawingLine line:

                    svg.AppendLine($"""
                <line x1="{line.X1}"
                      y1="{line.Y1}"
                      x2="{line.X2}"
                      y2="{line.Y2}"
                      stroke="{line.Stroke}"
                      stroke-width="{line.StrokeWidth}" />
                """);

                    break;

                case DrawingRectangle rectangle:

                    svg.AppendLine($"""
                <rect x="{rectangle.X}"
                      y="{rectangle.Y}"
                      width="{rectangle.Width}"
                      height="{rectangle.Height}"
                      fill="{rectangle.Fill}"
                      stroke="{rectangle.Stroke}"
                      stroke-width="{rectangle.StrokeWidth}" />
                """);

                    break;

                case DrawingCircle circle:

                    svg.AppendLine($"""
                <circle cx="{circle.CenterX}"
                        cy="{circle.CenterY}"
                        r="{circle.Radius}"
                        fill="{circle.Fill}"
                        stroke="{circle.Stroke}"
                        stroke-width="{circle.StrokeWidth}" />
                """);

                    break;
            }
        }

        private static void AddViewBorder(
            StringBuilder svg,
            DrawingView view)
        {
            svg.AppendLine($"""
        <rect x="{view.X}"
              y="{view.Y}"
              width="{view.Width}"
              height="{view.Height}"
              fill="none"
              stroke="#777777"
              stroke-width="1"
              stroke-dasharray="5,5" />
        """);
        }

        private static void AddText(
            StringBuilder svg,
            double x,
            double y,
            string text,
            double fontSize,
            string anchor = "start",
            bool bold = false)
        {
            var escapedText = SecurityElement.Escape(text);

            var fontWeight = bold ? "bold" : "normal";

            svg.AppendLine($"""
        <text x="{x}"
              y="{y}"
              font-family="Arial, sans-serif"
              font-size="{fontSize}"
              font-weight="{fontWeight}"
              text-anchor="{anchor}"
              fill="black">
            {escapedText}
        </text>
        """);
        }

        private static void RenderDimension(
            StringBuilder svg,
            DrawingDimension dimension)
        {
            var x1 = dimension.X1;
            var y1 = dimension.Y1;
            var x2 = dimension.X2;
            var y2 = dimension.Y2;

            // Main dimension line
            svg.AppendLine($"""
        <line x1="{x1}"
              y1="{y1}"
              x2="{x2}"
              y2="{y2}"
              stroke="black"
              stroke-width="1"/>
        """);

            // Extension lines
            svg.AppendLine($"""
        <line x1="{x1}"
              y1="{y1 - 15}"
              x2="{x1}"
              y2="{y1 + 15}"
              stroke="black"
              stroke-width="1"/>

        <line x1="{x2}"
              y1="{y2 - 15}"
              x2="{x2}"
              y2="{y2 + 15}"
              stroke="black"
              stroke-width="1"/>
        """);

            // Arrowheads
            AddArrow(svg, x1, y1, x2, y2);
            AddArrow(svg, x2, y2, x1, y1);

            // Label
            var labelX = (x1 + x2) / 2;
            var labelY = (y1 + y2) / 2 - dimension.Offset;

            AddText(
                svg,
                labelX,
                labelY,
                dimension.Label,
                11,
                "middle");
        }

        private static void AddArrow(
            StringBuilder svg,
            double x,
            double y,
            double targetX,
            double targetY)
        {
            var dx = targetX - x;
            var dy = targetY - y;

            var length = Math.Sqrt(dx * dx + dy * dy);

            if (length == 0)
                return;

            var ux = dx / length;
            var uy = dy / length;

            const double arrowLength = 8;
            const double arrowWidth = 4;

            var baseX = x + ux * arrowLength;
            var baseY = y + uy * arrowLength;

            var px = -uy;
            var py = ux;

            var leftX = baseX + px * arrowWidth;
            var leftY = baseY + py * arrowWidth;

            var rightX = baseX - px * arrowWidth;
            var rightY = baseY - py * arrowWidth;

            svg.AppendLine($"""
        <polygon points="
            {x},{y}
            {leftX},{leftY}
            {rightX},{rightY}"
            fill="black"/>
        """);
        }

        private static void RenderTechnicalData(
            StringBuilder svg,
            DrawingModel model)
        {
            var data = model.TechnicalData;

            const double x = 800;
            const double y = 50;
            const double width = 300;
            const double height = 320;

            svg.AppendLine($"""
        <rect x="{x}"
              y="{y}"
              width="{width}"
              height="{height}"
              fill="white"
              stroke="black"
              stroke-width="1"/>
        """);

            AddText(
                svg,
                x + 10,
                y + 25,
                "TECHNICAL DATA",
                14,
                "start",
                true);

            var rowY = y + 55;

            AddTechnicalRow(svg, x, ref rowY, "Design", data.DesignName);
            AddTechnicalRow(svg, x, ref rowY, "Crane Type", data.CraneType);

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Capacity",
                $"{data.CapacityTonnes:0.##} tonnes");

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Span",
                $"{data.SpanMm:0} mm");

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Lift Height",
                $"{data.LiftHeightMm:0} mm");

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Runway Width",
                $"{data.RunwayWidthMm:0} mm");

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Hoist Position",
                $"{data.HoistPositionMm:0} mm");

            AddTechnicalRow(
                svg,
                x,
                ref rowY,
                "Building Height",
                $"{data.BuildingHeightMm:0} mm");
        }

        private static void AddTechnicalRow(
            StringBuilder svg,
            double x,
            ref double y,
            string label,
            string value)
        {
            AddText(
                svg,
                x + 10,
                y,
                label,
                11);

            AddText(
                svg,
                x + 285,
                y,
                value,
                11,
                "end");

            y += 30;
        }

        private static void RenderTitleBlock(
            StringBuilder svg,
            DrawingModel model)
        {
            const double x = 50;
            const double y = 650;
            const double width = 1050;
            const double height = 100;

            svg.AppendLine($"""
        <rect x="{x}"
              y="{y}"
              width="{width}"
              height="{height}"
              fill="white"
              stroke="black"
              stroke-width="2"/>
        """);

            AddText(
                svg,
                x + 20,
                y + 30,
                "PARAMETRIC OVERHEAD CRANE",
                14,
                "start",
                true);

            AddText(
                svg,
                x + 20,
                y + 55,
                "CONCEPTUAL DRAWING - NOT FOR FABRICATION",
                12,
                "start",
                true);

            AddText(
                svg,
                x + 20,
                y + 80,
                $"Drawing No: {model.DrawingNumber}",
                11);

            AddText(
                svg,
                x + 400,
                y + 80,
                $"Scale: {model.Scale}",
                11);

            AddText(
                svg,
                x + 700,
                y + 80,
                $"Revision: A",
                11);

            AddText(
                svg,
                x + 1030,
                y + 80,
                DateTime.Now.ToString("yyyy-MM-dd"),
                11,
                "end");
        }
    }
}
