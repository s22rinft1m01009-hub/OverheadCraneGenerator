using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Details;

public static class RunwayBayDiagramGenerator
{
    public static DrawingDetail Generate(
        double spanMm, double runwayWidthMm,
        double startX = 320, double startY = 570, double width = 275, double height = 190)
    {
        var bayDetail = new DrawingDetail
        {
            Identifier = "BAY LAYOUT",
            Name = "RUNWAY & BAY ARRANGEMENT",
            Scale = "1:250",
            X = startX,
            Y = startY,
            Width = width,
            Height = height
        };

        // Title
        bayDetail.Elements.Add(new DrawingText
        {
            X = startX + 10,
            Y = startY + 16,
            Content = "RUNWAY / BAY SCHEMATIC DIAGRAM",
            FontSize = 9.5,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#000000"
        });

        // Layout boundaries
        var bayLeft = startX + 45;
        var bayRight = startX + width - 35;
        var bayTop = startY + 35;
        var bayBottom = startY + height - 35;
        var craneY = (bayTop + bayBottom) * 0.52;

        // Building bay columns (dots/rectangles along left and right runway)
        for (var y = bayTop; y <= bayBottom; y += (bayBottom - bayTop) / 4)
        {
            // Left runway column
            bayDetail.Elements.Add(new DrawingRectangle
            {
                X = bayLeft - 6,
                Y = y - 4,
                Width = 8,
                Height = 8,
                Fill = "#555555",
                Stroke = "#222222",
                StrokeWidth = 1
            });

            // Right runway column
            bayDetail.Elements.Add(new DrawingRectangle
            {
                X = bayRight - 2,
                Y = y - 4,
                Width = 8,
                Height = 8,
                Fill = "#555555",
                Stroke = "#222222",
                StrokeWidth = 1
            });
        }

        // Left runway rail
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = bayLeft,
            Y1 = bayTop - 5,
            X2 = bayLeft,
            Y2 = bayBottom + 5,
            Stroke = "#000000",
            StrokeWidth = 2.5
        });

        // Right runway rail
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = bayRight,
            Y1 = bayTop - 5,
            X2 = bayRight,
            Y2 = bayBottom + 5,
            Stroke = "#000000",
            StrokeWidth = 2.5
        });

        // Crane bridge schematic (spanning left to right runway)
        bayDetail.Elements.Add(new DrawingRectangle
        {
            X = bayLeft,
            Y = craneY - 6,
            Width = bayRight - bayLeft,
            Height = 12,
            Fill = "#f4c542",
            Stroke = "#d4ac0d",
            StrokeWidth = 1.5
        });

        // Trolley position on bridge
        bayDetail.Elements.Add(new DrawingRectangle
        {
            X = (bayLeft + bayRight) / 2 - 8,
            Y = craneY - 8,
            Width = 16,
            Height = 16,
            Fill = "#2980b9",
            Stroke = "#1b4f72",
            StrokeWidth = 1.5
        });

        // Crane bridge travel arrows
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = (bayLeft + bayRight) / 2,
            Y1 = craneY - 18,
            X2 = (bayLeft + bayRight) / 2,
            Y2 = craneY - 32,
            Stroke = "#27ae60",
            StrokeWidth = 1.8
        });
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = (bayLeft + bayRight) / 2,
            Y1 = craneY + 18,
            X2 = (bayLeft + bayRight) / 2,
            Y2 = craneY + 32,
            Stroke = "#27ae60",
            StrokeWidth = 1.8
        });

        bayDetail.Elements.Add(new DrawingText
        {
            X = (bayLeft + bayRight) / 2 + 15,
            Y = craneY - 22,
            Content = "TRAVEL",
            FontSize = 7,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#27ae60"
        });

        // FEEDING POINT marker (at bottom left)
        bayDetail.Elements.Add(new DrawingCircle
        {
            CenterX = bayLeft,
            CenterY = bayBottom,
            Radius = 6,
            Fill = "#e74c3c",
            Stroke = "#922b21",
            StrokeWidth = 1.5
        });
        bayDetail.Elements.Add(new DrawingCircle
        {
            CenterX = bayLeft,
            CenterY = bayBottom,
            Radius = 2.5,
            Fill = "#ffffff",
            Stroke = "#922b21",
            StrokeWidth = 1
        });
        bayDetail.Elements.Add(new DrawingText
        {
            X = bayLeft + 10,
            Y = bayBottom + 4,
            Content = "FEEDING POINT",
            FontSize = 7.5,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#c0392b"
        });

        // 0 m mark
        bayDetail.Elements.Add(new DrawingText
        {
            X = bayLeft - 10,
            Y = bayBottom + 12,
            Content = "0 m",
            FontSize = 7.5,
            FontWeight = "bold",
            TextAnchor = "end",
            Fill = "#333333"
        });

        // Bay length mark
        var bayLengthMm = Math.Max(runwayWidthMm, 25000);
        bayDetail.Elements.Add(new DrawingText
        {
            X = bayLeft - 10,
            Y = bayTop - 2,
            Content = $"{bayLengthMm:N0} mm",
            FontSize = 7.5,
            FontWeight = "bold",
            TextAnchor = "end",
            Fill = "#333333"
        });

        // DIRECTION NORTH arrow
        var northX = bayRight - 15;
        var northY = bayTop + 20;
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = northX,
            Y1 = northY + 18,
            X2 = northX,
            Y2 = northY - 8,
            Stroke = "#000000",
            StrokeWidth = 1.8
        });
        // Arrowhead pointing UP
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = northX - 5,
            Y1 = northY - 2,
            X2 = northX,
            Y2 = northY - 8,
            Stroke = "#000000",
            StrokeWidth = 1.8
        });
        bayDetail.Elements.Add(new DrawingLine
        {
            X1 = northX + 5,
            Y1 = northY - 2,
            X2 = northX,
            Y2 = northY - 8,
            Stroke = "#000000",
            StrokeWidth = 1.8
        });
        bayDetail.Elements.Add(new DrawingText
        {
            X = northX,
            Y = northY - 11,
            Content = "N",
            FontSize = 9,
            FontWeight = "bold",
            TextAnchor = "middle",
            Fill = "#000000"
        });
        bayDetail.Elements.Add(new DrawingText
        {
            X = northX - 2,
            Y = northY + 30,
            Content = "DIRECTION NORTH",
            FontSize = 6.5,
            FontWeight = "normal",
            TextAnchor = "middle",
            Fill = "#444444"
        });

        // Span dimension between rails
        bayDetail.Dimensions.Add(new DrawingDimension
        {
            X1 = bayLeft,
            Y1 = bayBottom + 18,
            X2 = bayRight,
            Y2 = bayBottom + 18,
            Label = $"SPAN {spanMm:N0} mm",
            Offset = 6,
            IsReference = false,
            Orientation = "horizontal"
        });

        return bayDetail;
    }
}
