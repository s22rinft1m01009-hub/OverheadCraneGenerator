using System.Text.Json;
using CraneGenerator.Application.Calculations;
using CraneGenerator.Application.CraneModels;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.Drawing.Pdf.Fonts;
using CraneGenerator.Domain.Entities;
using Xunit;

namespace CraneGenerator.Tests;

public sealed class CraneCalculationTests
{
    private readonly CraneCalculationService _calculationService = new();

    [Fact]
    public void SameInputProducesSameCalculationResult()
    {
        var design = Design("SINGLE_GIRDER_EOT");

        Assert.Equal(_calculationService.Calculate(design), _calculationService.Calculate(design));
    }

    [Fact]
    public void SpanChangeUpdatesScaleAndDimensionLabel()
    {
        var generator = new SingleGirderEotGenerator();
        var firstDesign = Design("SINGLE_GIRDER_EOT");
        var changedDesign = Design("SINGLE_GIRDER_EOT");
        changedDesign.SpanMm = 30000;

        var firstModel = generator.Generate(firstDesign, _calculationService.Calculate(firstDesign));
        var changedModel = generator.Generate(changedDesign, _calculationService.Calculate(changedDesign));

        Assert.NotEqual(firstModel.Scale, changedModel.Scale);
        Assert.Contains(firstModel.Dimensions, dimension => dimension.Label == "25,000 mm");
        Assert.Contains(changedModel.Dimensions, dimension => dimension.Label == "30,000 mm");
    }

    [Fact]
    public void HoistPositionChangeMovesTrolley()
    {
        var generator = new SingleGirderEotGenerator();
        var leftDesign = Design("SINGLE_GIRDER_EOT");
        leftDesign.HoistPositionMm = 5000;
        var rightDesign = Design("SINGLE_GIRDER_EOT");
        rightDesign.HoistPositionMm = 15000;

        var leftModel = generator.Generate(leftDesign, _calculationService.Calculate(leftDesign));
        var rightModel = generator.Generate(rightDesign, _calculationService.Calculate(rightDesign));
        var leftHoistX = leftModel.Elements.OfType<DrawingRectangle>().Single(x => x.Fill == "#4f81bd").X;
        var rightHoistX = rightModel.Elements.OfType<DrawingRectangle>().Single(x => x.Fill == "#4f81bd").X;

        Assert.NotEqual(leftHoistX, rightHoistX);
    }

    [Fact]
    public void SingleGirderHasOnePrimaryGirderAndDoubleGirderHasTwo()
    {
        var single = Generate("SINGLE_GIRDER_EOT", new SingleGirderEotGenerator());
        var doubleGirder = Generate("DOUBLE_GIRDER_EOT", new DoubleGirderEotGenerator());

        Assert.Equal(1, PrimaryGirders(single));
        Assert.Equal(2, PrimaryGirders(doubleGirder));
    }

    [Fact]
    public void GantryContainsFreestandingLegsAndBaseStructures()
    {
        var gantry = Generate("GANTRY", new GantryGenerator());

        Assert.Contains(gantry.Elements.OfType<DrawingLine>(), line => line.StrokeWidth == 4 && line.X1 != line.X2);
        Assert.True(gantry.Elements.OfType<DrawingRectangle>().Count(rectangle => rectangle.Height == 12) >= 2);
        Assert.DoesNotContain(gantry.Elements.OfType<DrawingRectangle>(), rectangle => rectangle.Fill == "#eeeeee" && rectangle.Height > 100);
    }

    [Fact]
    public void DrawingElementJsonIncludesDerivedGeometry()
    {
        var model = new DrawingModel
        {
            Elements = [new DrawingLine { X1 = 1, Y1 = 2, X2 = 3, Y2 = 4 }]
        };

        var json = JsonSerializer.Serialize(model);

        Assert.Contains("\"x1\":1", json);
        Assert.Contains("\"y2\":4", json);
        Assert.Contains("\"$type\":\"line\"", json);
    }

    [Fact]
    public void SvgContainsAllViewTitlesAndConceptualNotice()
    {
        var model = Generate("SINGLE_GIRDER_EOT", new SingleGirderEotGenerator());
        var svg = new SvgDrawingRenderer().Render(model);

        Assert.Contains("<svg", svg);
        Assert.Contains("MAIN ELEVATION", svg);
        Assert.Contains("PLAN VIEW", svg);
        Assert.Contains("SECTION A-A", svg);
        Assert.Contains("CONCEPTUAL DRAWING - NOT FOR FABRICATION", svg);
    }

    [Fact]
    public void PdfRendererReturnsPdfDocumentBytesForDrawingModel()
    {
        CraneFontResolver.Register();
        var model = Generate("DOUBLE_GIRDER_EOT", new DoubleGirderEotGenerator());
        var pdf = new PdfDrawingRenderer().Render(model);

        Assert.True(pdf.Length > 500);
        Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(pdf, 0, 5));
    }

    private DrawingModel Generate(string craneType, ICraneGeometryGenerator generator)
    {
        var design = Design(craneType);
        return generator.Generate(design, _calculationService.Calculate(design));
    }

    private static int PrimaryGirders(DrawingModel model) => model.Elements
        .OfType<DrawingRectangle>()
        .Count(rectangle => rectangle.Fill == "#f4c542" && rectangle.Width > 500 && rectangle.Y < 350);

    private static DesignResponse Design(string craneType) => new()
    {
        Id = 1,
        DesignName = "Test Crane",
        CraneType = craneType,
        CapacityTonnes = 10,
        SpanMm = 25000,
        LiftHeightMm = 8000,
        RunwayWidthMm = 12500,
        HoistPositionMm = 12500,
        BuildingHeightMm = craneType == "GANTRY" ? 0 : 9000
    };
}