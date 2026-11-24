using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Sheet;

public static class BorderRenderer
{
    public static void AddBorders(DrawingModel model)
    {
        var meta = model.SheetMetadata;
        var w = meta.SheetWidth;
        var h = meta.SheetHeight;
        var outerMargin = meta.OuterMargin;
        var innerMargin = meta.InnerMargin;

        // Outer border
        model.Elements.Add(new DrawingRectangle
        {
            X = outerMargin,
            Y = outerMargin,
            Width = w - outerMargin * 2,
            Height = h - outerMargin * 2,
            Fill = "none",
            Stroke = "#000000",
            StrokeWidth = 1.8
        });

        // Inner border (CAD drawing area)
        model.Elements.Add(new DrawingRectangle
        {
            X = innerMargin,
            Y = innerMargin,
            Width = w - innerMargin * 2,
            Height = h - innerMargin * 2,
            Fill = "none",
            Stroke = "#000000",
            StrokeWidth = 1.2
        });

        // Top warning / status banner bar
        model.Elements.Add(new DrawingRectangle
        {
            X = innerMargin,
            Y = innerMargin,
            Width = w - innerMargin * 2,
            Height = 22,
            Fill = "#fff8e1",
            Stroke = "#000000",
            StrokeWidth = 1
        });

        // Warning banner text
        model.Elements.Add(new DrawingText
        {
            X = w / 2,
            Y = innerMargin + 15,
            Content = meta.WarningNotice,
            FontSize = 8.5,
            FontWeight = "bold",
            TextAnchor = "middle",
            Fill = "#b71c1c"
        });
    }
}
