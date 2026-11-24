using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Application.Drawing.Details;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Sheet;

public static class DrawingSheetRenderer
{
    public static void AssembleSheet(
        DrawingModel model,
        DesignResponse design,
        CraneCalculationResult calculation)
    {
        // 1. Sheet metadata
        model.SheetMetadata = new DrawingSheetMetadata
        {
            SheetWidth = 1200,
            SheetHeight = 800,
            OuterMargin = 15,
            InnerMargin = 30,
            WarningNotice = "CONCEPTUAL DRAWING - NOT FOR FABRICATION - FOR PRELIMINARY REVIEW ONLY",
            ProjectTitle = "PARAMETRIC OVERHEAD CRANE"
        };

        // 2. CAD Border
        BorderRenderer.AddBorders(model);

        // 3. CAD Grid Zone references (A..H, 1..6)
        GridRenderer.AddGrid(model);

        // 4. Detail Z (Rail / Wheel Interface)
        var railWheelDetail = RailWheelDetailGenerator.Generate(startX: 40, startY: 570, width: 265, height: 190);
        model.Details.Add(railWheelDetail);

        // 5. Runway / Bay Diagram
        var bayDiagram = RunwayBayDiagramGenerator.Generate(
            spanMm: design.SpanMm,
            runwayWidthMm: design.RunwayWidthMm,
            startX: 315,
            startY: 570,
            width: 285,
            height: 190);
        model.Details.Add(bayDiagram);

        // 6. End Stop Detail & Engineering Notes
        var endStopDetail = EndStopDetailGenerator.Generate(startX: 615, startY: 665, width: 265, height: 95);
        model.Details.Add(endStopDetail);

        // 7. Title Block and Revision Table
        TitleBlockRenderer.AddTitleBlockAndRevisions(model);

        // 8. Add general notes to the model
        model.Notes.Add(new DrawingNote { Number = 1, Text = "ALL DIMENSIONS ARE IN MILLIMETERS (mm) UNLESS OTHERWISE SPECIFIED.", Category = "GENERAL" });
        model.Notes.Add(new DrawingNote { Number = 2, Text = "CRANE END STOP SUPPLIED BY CUSTOMER ACCORDING TO APPLICABLE SAFETY STANDARDS.", Category = "INSTALLATION" });
        model.Notes.Add(new DrawingNote { Number = 3, Text = "VALUES MARKED [ESTIMATE] OR [PROTOTYPE] ARE FOR CONCEPTUAL SIZING ONLY.", Category = "ENGINEERING" });
        model.Notes.Add(new DrawingNote { Number = 4, Text = "FABRICATION AND SITE INSTALLATION REQUIRE DETAILED STRUCTURAL VERIFICATION.", Category = "SAFETY" });

        // 9. Add initial revision
        model.Revisions.Add(new DrawingRevision
        {
            Rev = "A",
            Description = "Initial conceptual parametric drawing",
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Designed = "AGY",
            Checked = "ENG"
        });
    }
}
