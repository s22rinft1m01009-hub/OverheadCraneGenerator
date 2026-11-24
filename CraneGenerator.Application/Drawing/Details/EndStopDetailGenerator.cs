using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Details;

public static class EndStopDetailGenerator
{
    public static DrawingDetail Generate(double startX = 605, double startY = 665, double width = 265, double height = 95)
    {
        var detail = new DrawingDetail
        {
            Identifier = "NOTES & END STOP",
            Name = "RUNWAY END STOP SPECIFICATION",
            Scale = "NTS",
            X = startX,
            Y = startY,
            Width = width,
            Height = height
        };

        // Header text
        detail.Elements.Add(new DrawingText
        {
            X = startX + 10,
            Y = startY + 16,
            Content = "ENGINEERING NOTES & CRANE END STOPS",
            FontSize = 9.5,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#000000"
        });

        // End stop callout box
        detail.Elements.Add(new DrawingRectangle
        {
            X = startX + 10,
            Y = startY + 24,
            Width = width - 20,
            Height = 22,
            Fill = "#fff9c4",
            Stroke = "#f57f17",
            StrokeWidth = 1
        });

        detail.Elements.Add(new DrawingText
        {
            X = startX + 15,
            Y = startY + 38,
            Content = "CRANE END STOP SUPPLIED BY CUSTOMER",
            FontSize = 8.5,
            FontWeight = "bold",
            TextAnchor = "start",
            Fill = "#b71c1c"
        });

        // Notes list
        var notes = new[]
        {
            "1. End stops must withstand full buffer impact at 100% rated travel speed.",
            "2. All dimensions & clearances marked [ESTIMATE] require site verification.",
            "3. Rail alignment tolerance acc. to ISO 12488-1 Class 2."
        };

        var curY = startY + 58;
        foreach (var note in notes)
        {
            detail.Elements.Add(new DrawingText
            {
                X = startX + 10,
                Y = curY,
                Content = note,
                FontSize = 7,
                FontWeight = "normal",
                TextAnchor = "start",
                Fill = "#222222"
            });
            curY += 12;
        }

        return detail;
    }
}
