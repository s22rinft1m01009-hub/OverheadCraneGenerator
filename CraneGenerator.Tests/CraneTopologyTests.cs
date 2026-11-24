using CraneGenerator.Application.Calculations;
using CraneGenerator.Application.CraneModels;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.Drawing.Pdf.Fonts;
using CraneGenerator.Domain.Entities;
using CraneGenerator.Domain.Enums;
using System.Linq;
using Xunit;

namespace CraneGenerator.Tests;

public sealed class CraneTopologyTests
{
    private readonly CraneCalculationService _calculationService = new();
    private readonly CraneDrawingFactory _factory;
    private readonly SvgDrawingRenderer _svgRenderer = new();
    private readonly PdfDrawingRenderer _pdfRenderer = new();

    public CraneTopologyTests()
    {
        CraneFontResolver.Register();

        _factory = new CraneDrawingFactory(
            new SingleGirderEotGenerator(),
            new DoubleGirderEotGenerator(),
            new GantryGenerator(),
            new JibCraneGenerator(),
            new MonorailGenerator(),
            new WallMountedJibGenerator(),
            new PortableGantryGenerator(),
            new SemiGantryGenerator(),
            new UnderslungGenerator(),
            _calculationService);
    }

    [Theory]
    [InlineData("SINGLE_GIRDER_EOT")]
    [InlineData("DOUBLE_GIRDER_EOT")]
    [InlineData("GANTRY")]
    [InlineData("JIB_CRANE")]
    [InlineData("MONORAIL")]
    [InlineData("WALL_MOUNTED_JIB")]
    [InlineData("PORTABLE_GANTRY")]
    [InlineData("SEMI_GANTRY")]
    [InlineData("UNDERSLUNG")]
    public void AllNineCraneTypesGenerateValidModelsViewsAndElements(string craneType)
    {
        var design = CreateDesign(craneType);
        var model = _factory.Generate(design);

        Assert.NotNull(model);
        Assert.NotEmpty(model.Elements);
        Assert.NotEmpty(model.Dimensions);
        Assert.Equal(3, model.Views.Count); // MAIN ELEVATION, PLAN VIEW, SECTION A-A
        Assert.Contains(model.Views, v => v.Name == "MAIN ELEVATION");
        Assert.Contains(model.Views, v => v.Name == "PLAN VIEW");
        Assert.Contains(model.Views, v => v.Name == "SECTION A-A");

        // Verify CAD Details are populated
        Assert.NotEmpty(model.Details);
        Assert.Contains(model.Details, d => d.Identifier == "DETAIL Z");
        Assert.Contains(model.Details, d => d.Identifier == "BAY LAYOUT");
        Assert.Contains(model.Details, d => d.Identifier == "NOTES & END STOP");

        // Verify Technical Specifications
        Assert.NotNull(model.TechnicalData);
        Assert.NotEmpty(model.TechnicalData.Specifications);
        Assert.Contains(model.TechnicalData.Specifications, s => s.Source == SpecificationSource.UserInput);
        Assert.Contains(model.TechnicalData.Specifications, s => s.Source == SpecificationSource.Estimate);

        // Verify Sheet Metadata & Notes
        Assert.NotEmpty(model.Notes);
        Assert.NotNull(model.SheetMetadata);
        Assert.NotEmpty(model.SheetMetadata.WarningNotice);
    }

    [Fact]
    public void DistinctCraneTypesProduceDistinctGeometry()
    {
        var singleGirder = _factory.Generate(CreateDesign("SINGLE_GIRDER_EOT"));
        var doubleGirder = _factory.Generate(CreateDesign("DOUBLE_GIRDER_EOT"));
        var gantry = _factory.Generate(CreateDesign("GANTRY"));
        var jib = _factory.Generate(CreateDesign("JIB_CRANE"));
        var monorail = _factory.Generate(CreateDesign("MONORAIL"));
        var wallJib = _factory.Generate(CreateDesign("WALL_MOUNTED_JIB"));
        var portableGantry = _factory.Generate(CreateDesign("PORTABLE_GANTRY"));
        var semiGantry = _factory.Generate(CreateDesign("SEMI_GANTRY"));
        var underslung = _factory.Generate(CreateDesign("UNDERSLUNG"));

        // Single Girder vs Double Girder
        Assert.NotEqual(singleGirder.Elements.Count, doubleGirder.Elements.Count);

        // Double Girder vs Gantry
        Assert.NotEqual(doubleGirder.Elements.Count, gantry.Elements.Count);

        // Gantry vs Jib Crane (Jib has arcs for rotation)
        Assert.True(jib.Elements.OfType<DrawingArc>().Any());

        // Jib vs Monorail
        Assert.NotEqual(jib.Elements.Count, monorail.Elements.Count);

        // Wall Mounted Jib vs Portable Gantry
        Assert.NotEqual(wallJib.Elements.Count, portableGantry.Elements.Count);

        // Semi Gantry vs Underslung
        Assert.NotEqual(semiGantry.Elements.Count, underslung.Elements.Count);
    }

    [Fact]
    public void AllCraneTypesProduceValidSvgAndPdf()
    {
        var types = new[]
        {
            "SINGLE_GIRDER_EOT", "DOUBLE_GIRDER_EOT", "GANTRY", "JIB_CRANE",
            "MONORAIL", "WALL_MOUNTED_JIB", "PORTABLE_GANTRY", "SEMI_GANTRY", "UNDERSLUNG"
        };

        foreach (var type in types)
        {
            var design = CreateDesign(type);
            var model = _factory.Generate(design);

            // SVG Test
            var svg = _svgRenderer.Render(model);
            Assert.Contains("<svg", svg);
            Assert.Contains("</svg>", svg);
            Assert.Contains("MAIN ELEVATION", svg);
            Assert.Contains("PLAN VIEW", svg);
            Assert.Contains("SECTION A-A", svg);
            Assert.Contains("DETAIL Z", svg);
            Assert.Contains("TECHNICAL SPECIFICATIONS", svg);

            // PDF Test
            var pdf = _pdfRenderer.Render(model);
            Assert.NotNull(pdf);
            Assert.True(pdf.Length > 1000);
            Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(pdf, 0, 5));
        }
    }

    private static DesignResponse CreateDesign(string type) => new()
    {
        Id = 1,
        DesignName = $"Test {type}",
        CraneType = type,
        CapacityTonnes = 12.5,
        SpanMm = 20000,
        LiftHeightMm = 8000,
        RunwayWidthMm = 15000,
        HoistPositionMm = 10000,
        BuildingHeightMm = 9500
    };
}
