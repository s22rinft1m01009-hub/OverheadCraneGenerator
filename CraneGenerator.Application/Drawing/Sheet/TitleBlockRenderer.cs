using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Sheet;

public static class TitleBlockRenderer
{
    public static void AddTitleBlockAndRevisions(DrawingModel model)
    {
        const double titleX = 890;
        const double titleY = 570;
        const double titleW = 275;
        const double titleH = 190;

        // Title Block Outer Frame
        model.Elements.Add(new DrawingRectangle
        {
            X = titleX, Y = titleY,
            Width = titleW, Height = titleH,
            Fill = "#ffffff", Stroke = "#000000", StrokeWidth = 1.5
        });

        // Title Block Header Bar
        model.Elements.Add(new DrawingRectangle
        {
            X = titleX, Y = titleY,
            Width = titleW, Height = 22,
            Fill = "#e8f8f5", Stroke = "#000000", StrokeWidth = 1
        });
        model.Elements.Add(new DrawingText
        {
            X = titleX + titleW / 2, Y = titleY + 15,
            Content = "PARAMETRIC OVERHEAD CRANE",
            FontSize = 10, FontWeight = "bold",
            TextAnchor = "middle", Fill = "#117a65"
        });

        // Drawing Title Box
        model.Elements.Add(new DrawingRectangle
        {
            X = titleX, Y = titleY + 22,
            Width = titleW, Height = 32,
            Fill = "none", Stroke = "#000000", StrokeWidth = 1
        });
        model.Elements.Add(new DrawingText
        {
            X = titleX + 8, Y = titleY + 34,
            Content = "TITLE / CRANE TYPE:",
            FontSize = 7, FontWeight = "normal",
            TextAnchor = "start", Fill = "#555555"
        });
        model.Elements.Add(new DrawingText
        {
            X = titleX + 8, Y = titleY + 48,
            Content = model.TechnicalData.CraneType.Replace('_', ' '),
            FontSize = 9.5, FontWeight = "bold",
            TextAnchor = "start", Fill = "#000000"
        });

        // Parameters Grid: Drawing Number, Revision, Scale, Date
        const double rowH = 18;
        var rY = titleY + 54;

        // Drawing No. row
        model.Elements.Add(new DrawingLine { X1 = titleX, Y1 = rY, X2 = titleX + titleW, Y2 = rY, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingText { X = titleX + 8, Y = rY + 12, Content = "DRAWING NO:", FontSize = 7.5, FontWeight = "normal", Fill = "#555555" });
        model.Elements.Add(new DrawingText { X = titleX + 110, Y = rY + 12, Content = model.DrawingNumber, FontSize = 8.5, FontWeight = "bold", Fill = "#000000" });

        // Scale & Rev row
        rY += rowH;
        model.Elements.Add(new DrawingLine { X1 = titleX, Y1 = rY, X2 = titleX + titleW, Y2 = rY, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingLine { X1 = titleX + 140, Y1 = rY, X2 = titleX + 140, Y2 = rY + rowH, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingText { X = titleX + 8, Y = rY + 12, Content = "SCALE: " + model.Scale, FontSize = 7.5, FontWeight = "bold", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = titleX + 148, Y = rY + 12, Content = "REV: " + model.Revision, FontSize = 7.5, FontWeight = "bold", Fill = "#000000" });

        // Date & Units row
        rY += rowH;
        model.Elements.Add(new DrawingLine { X1 = titleX, Y1 = rY, X2 = titleX + titleW, Y2 = rY, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingLine { X1 = titleX + 140, Y1 = rY, X2 = titleX + 140, Y2 = rY + rowH, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingText { X = titleX + 8, Y = rY + 12, Content = "DATE: " + DateTime.UtcNow.ToString("yyyy-MM-dd"), FontSize = 7.5, FontWeight = "normal", Fill = "#222222" });
        model.Elements.Add(new DrawingText { X = titleX + 148, Y = rY + 12, Content = "UNITS: mm", FontSize = 7.5, FontWeight = "bold", Fill = "#222222" });

        // Approvals Table (DESIGNED, CHECKED, VERIFIED, APPROVED)
        rY += rowH;
        model.Elements.Add(new DrawingLine { X1 = titleX, Y1 = rY, X2 = titleX + titleW, Y2 = rY, Stroke = "#000000", StrokeWidth = 0.8 });
        
        var colW = titleW / 4;
        var approvalRoles = new[] { "DESIGNED", "CHECKED", "VERIFIED", "APPROVED" };
        var approvalNames = new[] { "AGY", "ENG", "-", "-" };

        for (var i = 0; i < 4; i++)
        {
            var colX = titleX + i * colW;
            if (i > 0)
                model.Elements.Add(new DrawingLine { X1 = colX, Y1 = rY, X2 = colX, Y2 = rY + 28, Stroke = "#000000", StrokeWidth = 0.8 });

            model.Elements.Add(new DrawingText { X = colX + colW / 2, Y = rY + 11, Content = approvalRoles[i], FontSize = 6.5, FontWeight = "bold", TextAnchor = "middle", Fill = "#555555" });
            model.Elements.Add(new DrawingText { X = colX + colW / 2, Y = rY + 23, Content = approvalNames[i], FontSize = 7.5, FontWeight = "normal", TextAnchor = "middle", Fill = "#000000" });
        }

        // Material / Code / Status Box
        rY += 28;
        model.Elements.Add(new DrawingLine { X1 = titleX, Y1 = rY, X2 = titleX + titleW, Y2 = rY, Stroke = "#000000", StrokeWidth = 0.8 });
        model.Elements.Add(new DrawingText { X = titleX + 8, Y = rY + 12, Content = "MATERIAL: S355 / REF [ESTIMATE]", FontSize = 7, FontWeight = "normal", Fill = "#444444" });
        model.Elements.Add(new DrawingText { X = titleX + 8, Y = rY + 24, Content = "STATUS: CONCEPTUAL / OFFER ONLY", FontSize = 7, FontWeight = "bold", Fill = "#b71c1c" });

        // Revision Table (adjacent to Title Block)
        const double revX = 615;
        const double revY = 570;
        const double revW = 265;
        const double revH = 88;

        model.Elements.Add(new DrawingRectangle
        {
            X = revX, Y = revY,
            Width = revW, Height = revH,
            Fill = "#ffffff", Stroke = "#000000", StrokeWidth = 1.2
        });

        // Revision Table Header
        model.Elements.Add(new DrawingRectangle
        {
            X = revX, Y = revY,
            Width = revW, Height = 18,
            Fill = "#f2f4f4", Stroke = "#000000", StrokeWidth = 0.8
        });

        model.Elements.Add(new DrawingText { X = revX + 15, Y = revY + 12, Content = "REV", FontSize = 7, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 90, Y = revY + 12, Content = "DESCRIPTION", FontSize = 7, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 175, Y = revY + 12, Content = "DATE", FontSize = 7, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 220, Y = revY + 12, Content = "BY", FontSize = 7, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 250, Y = revY + 12, Content = "CHK", FontSize = 7, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });

        // Vertical divider lines in revision table
        model.Elements.Add(new DrawingLine { X1 = revX + 30, Y1 = revY, X2 = revX + 30, Y2 = revY + revH, Stroke = "#000000", StrokeWidth = 0.6 });
        model.Elements.Add(new DrawingLine { X1 = revX + 150, Y1 = revY, X2 = revX + 150, Y2 = revY + revH, Stroke = "#000000", StrokeWidth = 0.6 });
        model.Elements.Add(new DrawingLine { X1 = revX + 205, Y1 = revY, X2 = revX + 205, Y2 = revY + revH, Stroke = "#000000", StrokeWidth = 0.6 });
        model.Elements.Add(new DrawingLine { X1 = revX + 235, Y1 = revY, X2 = revX + 235, Y2 = revY + revH, Stroke = "#000000", StrokeWidth = 0.6 });

        // Initial Revision Row
        var revRowY = revY + 18;
        model.Elements.Add(new DrawingLine { X1 = revX, Y1 = revRowY + 18, X2 = revX + revW, Y2 = revRowY + 18, Stroke = "#cccccc", StrokeWidth = 0.6 });

        model.Elements.Add(new DrawingText { X = revX + 15, Y = revRowY + 12, Content = "A", FontSize = 7.5, FontWeight = "bold", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 35, Y = revRowY + 12, Content = "INITIAL CONCEPTUAL DRAWING", FontSize = 6.5, FontWeight = "normal", TextAnchor = "start", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 177, Y = revRowY + 12, Content = DateTime.UtcNow.ToString("yyyy-MM-dd"), FontSize = 6.5, FontWeight = "normal", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 220, Y = revRowY + 12, Content = "AGY", FontSize = 6.5, FontWeight = "normal", TextAnchor = "middle", Fill = "#000000" });
        model.Elements.Add(new DrawingText { X = revX + 250, Y = revRowY + 12, Content = "ENG", FontSize = 6.5, FontWeight = "normal", TextAnchor = "middle", Fill = "#000000" });
    }
}
