using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Details;

public static class RailWheelDetailGenerator
{
    public static DrawingDetail Generate(double startX = 45, double startY = 570, double width = 265, double height = 190)
    {
        var detail = new DrawingDetail
        {
            Identifier = "DETAIL Z",
            Name = "RAIL / WHEEL INTERFACE",
            Scale = "1:5",
            X = startX,
            Y = startY,
            Width = width,
            Height = height
        };

        var cx = startX + width / 2;
        var railTop = startY + 115;
        const double railHeadW = 46;
        const double railHeadH = 22;
        const double railWebW = 18;
        const double railWebH = 32;
        const double railBaseW = 76;
        const double railBaseH = 14;

        // Runway beam top flange under rail
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - 70,
            Y = railTop + railHeadH + railWebH + railBaseH,
            Width = 140,
            Height = 12,
            Fill = "#dddddd",
            Stroke = "#333333",
            StrokeWidth = 1.5
        });

        // Sole plate / rubber pad
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - 50,
            Y = railTop + railHeadH + railWebH + railBaseH - 3,
            Width = 100,
            Height = 3,
            Fill = "#555555",
            Stroke = "#333333",
            StrokeWidth = 1
        });

        // Crane Rail Base
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - railBaseW / 2,
            Y = railTop + railHeadH + railWebH,
            Width = railBaseW,
            Height = railBaseH,
            Fill = "#bbbbbb",
            Stroke = "#222222",
            StrokeWidth = 1.5
        });

        // Crane Rail Web
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - railWebW / 2,
            Y = railTop + railHeadH,
            Width = railWebW,
            Height = railWebH,
            Fill = "#bbbbbb",
            Stroke = "#222222",
            StrokeWidth = 1.5
        });

        // Crane Rail Head
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - railHeadW / 2,
            Y = railTop,
            Width = railHeadW,
            Height = railHeadH,
            Fill = "#cccccc",
            Stroke = "#111111",
            StrokeWidth = 1.5
        });

        // Rail clamps / clips on left and right
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx - railBaseW / 2 - 12,
            Y = railTop + railHeadH + railWebH + 2,
            Width = 14,
            Height = railBaseH + 2,
            Fill = "#777777",
            Stroke = "#333333",
            StrokeWidth = 1
        });
        detail.Elements.Add(new DrawingRectangle
        {
            X = cx + railBaseW / 2 - 2,
            Y = railTop + railHeadH + railWebH + 2,
            Width = 14,
            Height = railBaseH + 2,
            Fill = "#777777",
            Stroke = "#333333",
            StrokeWidth = 1
        });

        // Double-flanged Crane Wheel
        const double wheelR = 48;
        var wheelCenterY = railTop - wheelR + 4; // slight contact penetration for tread

        // Wheel main body
        detail.Elements.Add(new DrawingCircle
        {
            CenterX = cx,
            CenterY = wheelCenterY,
            Radius = wheelR,
            Fill = "#d4e6f1",
            Stroke = "#1b4f72",
            StrokeWidth = 2
        });

        // Wheel inner hub / axle
        detail.Elements.Add(new DrawingCircle
        {
            CenterX = cx,
            CenterY = wheelCenterY,
            Radius = 16,
            Fill = "#a9cce3",
            Stroke = "#154360",
            StrokeWidth = 1.5
        });
        detail.Elements.Add(new DrawingCircle
        {
            CenterX = cx,
            CenterY = wheelCenterY,
            Radius = 7,
            Fill = "#2980b9",
            Stroke = "#0e2f44",
            StrokeWidth = 1
        });

        // Centerlines
        detail.Elements.Add(new DrawingCenterline
        {
            X1 = cx,
            Y1 = wheelCenterY - wheelR - 10,
            X2 = cx,
            Y2 = railTop + railHeadH + railWebH + railBaseH + 18,
            StrokeWidth = 0.8
        });
        detail.Elements.Add(new DrawingCenterline
        {
            X1 = cx - wheelR - 10,
            Y1 = wheelCenterY,
            X2 = cx + wheelR + 10,
            Y2 = wheelCenterY,
            StrokeWidth = 0.8
        });

        // Dimensions
        // Wheel diameter callout
        detail.Dimensions.Add(new DrawingDimension
        {
            X1 = cx - wheelR,
            Y1 = wheelCenterY,
            X2 = cx + wheelR,
            Y2 = wheelCenterY,
            Label = "Ø250 [ESTIMATE]",
            Offset = 14,
            IsReference = true,
            Orientation = "horizontal"
        });

        // Rail head width
        detail.Dimensions.Add(new DrawingDimension
        {
            X1 = cx - railHeadW / 2,
            Y1 = railTop + railHeadH + 2,
            X2 = cx + railHeadW / 2,
            Y2 = railTop + railHeadH + 2,
            Label = "50",
            Offset = 8,
            IsReference = true,
            Orientation = "horizontal"
        });

        // Rail total height
        detail.Dimensions.Add(new DrawingDimension
        {
            X1 = cx + railBaseW / 2 + 18,
            Y1 = railTop,
            X2 = cx + railBaseW / 2 + 18,
            Y2 = railTop + railHeadH + railWebH + railBaseH,
            Label = "75",
            Offset = 8,
            IsReference = true,
            Orientation = "vertical"
        });

        // Rail base flange height
        detail.Dimensions.Add(new DrawingDimension
        {
            X1 = cx - railBaseW / 2 - 20,
            Y1 = railTop + railHeadH + railWebH,
            X2 = cx - railBaseW / 2 - 20,
            Y2 = railTop + railHeadH + railWebH + railBaseH,
            Label = "30",
            Offset = 6,
            IsReference = true,
            Orientation = "vertical"
        });

        // Annotation texts
        detail.Elements.Add(new DrawingText
        {
            X = startX + 10,
            Y = startY + 18,
            Content = "DETAIL Z - RAIL / WHEEL INTERFACE",
            FontSize = 10,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#000000"
        });

        detail.Elements.Add(new DrawingText
        {
            X = startX + 10,
            Y = startY + 30,
            Content = "RAIL: QU-70 DIN 536 / WHEEL: Ø250 FORGED [PROTOTYPE]",
            FontSize = 7.5,
            FontWeight = "normal",
            TextAnchor = "start",
            Fill = "#444444"
        });

        return detail;
    }
}
