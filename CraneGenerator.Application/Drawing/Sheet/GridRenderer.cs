using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Sheet;

public static class GridRenderer
{
    public static void AddGrid(DrawingModel model)
    {
        var meta = model.SheetMetadata;
        var outerMargin = meta.OuterMargin;
        var innerMargin = meta.InnerMargin;
        var w = meta.SheetWidth;
        var h = meta.SheetHeight;

        var cols = meta.GridColumns.Split(',');
        var colWidth = (w - innerMargin * 2) / cols.Length;

        // Horizontal zone divisions (A - H) along top and bottom
        for (var i = 0; i < cols.Length; i++)
        {
            var x = innerMargin + i * colWidth;
            var labelX = x + colWidth / 2;

            // Tick lines
            if (i > 0)
            {
                // Top tick
                model.Elements.Add(new DrawingLine
                {
                    X1 = x, Y1 = outerMargin,
                    X2 = x, Y2 = innerMargin,
                    Stroke = "#000000", StrokeWidth = 0.8
                });
                // Bottom tick
                model.Elements.Add(new DrawingLine
                {
                    X1 = x, Y1 = h - innerMargin,
                    X2 = x, Y2 = h - outerMargin,
                    Stroke = "#000000", StrokeWidth = 0.8
                });
            }

            // Top letter
            model.Elements.Add(new DrawingText
            {
                X = labelX, Y = outerMargin + 10,
                Content = cols[i].Trim(),
                FontSize = 7.5, FontWeight = "bold",
                TextAnchor = "middle", Fill = "#333333"
            });

            // Bottom letter
            model.Elements.Add(new DrawingText
            {
                X = labelX, Y = h - outerMargin - 3,
                Content = cols[i].Trim(),
                FontSize = 7.5, FontWeight = "bold",
                TextAnchor = "middle", Fill = "#333333"
            });
        }

        var rows = meta.GridRows.Split(',');
        var rowHeight = (h - innerMargin * 2) / rows.Length;

        // Vertical zone divisions (1 - 6) along left and right
        for (var i = 0; i < rows.Length; i++)
        {
            var y = innerMargin + i * rowHeight;
            var labelY = y + rowHeight / 2 + 3;

            // Tick lines
            if (i > 0)
            {
                // Left tick
                model.Elements.Add(new DrawingLine
                {
                    X1 = outerMargin, Y1 = y,
                    X2 = innerMargin, Y2 = y,
                    Stroke = "#000000", StrokeWidth = 0.8
                });
                // Right tick
                model.Elements.Add(new DrawingLine
                {
                    X1 = w - innerMargin, Y1 = y,
                    X2 = w - outerMargin, Y2 = y,
                    Stroke = "#000000", StrokeWidth = 0.8
                });
            }

            // Left number
            model.Elements.Add(new DrawingText
            {
                X = outerMargin + 7, Y = labelY,
                Content = rows[i].Trim(),
                FontSize = 7.5, FontWeight = "bold",
                TextAnchor = "middle", Fill = "#333333"
            });

            // Right number
            model.Elements.Add(new DrawingText
            {
                X = w - outerMargin - 7, Y = labelY,
                Content = rows[i].Trim(),
                FontSize = 7.5, FontWeight = "bold",
                TextAnchor = "middle", Fill = "#333333"
            });
        }
    }
}
