using CraneGenerator.Application.Calculations;
using CraneGenerator.Application.CraneModels;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.Drawing.Pdf.Fonts;
using CraneGenerator.Domain.Entities;
using System.Linq;
using Xunit;

namespace CraneGenerator.Tests;

public sealed class ParametricVariationTests
{
    private readonly CraneCalculationService _calculationService = new();
    private readonly SingleGirderEotGenerator _generator = new();
    private readonly SvgDrawingRenderer _svgRenderer = new();
    private readonly PdfDrawingRenderer _pdfRenderer = new();

    public ParametricVariationTests()
    {
        CraneFontResolver.Register();
    }

    [Fact]
    public void DifferentDesignInputsProduceDifferentDimensionsAndGeometry()
    {
        // Design A
        var designA = new DesignResponse
        {
            Id = 1,
            DesignName = "Design A (Compact)",
            CraneType = "SINGLE_GIRDER_EOT",
            CapacityTonnes = 5,
            SpanMm = 18000,
            LiftHeightMm = 6000,
            BuildingHeightMm = 8000,
            HoistPositionMm = 9000,
            RunwayWidthMm = 10000
        };

        // Design B
        var designB = new DesignResponse
        {
            Id = 2,
            DesignName = "Design B (Heavy Extended)",
            CraneType = "SINGLE_GIRDER_EOT",
            CapacityTonnes = 25,
            SpanMm = 30000,
            LiftHeightMm = 10000,
            BuildingHeightMm = 12000,
            HoistPositionMm = 15000,
            RunwayWidthMm = 20000
        };

        var modelA = _generator.Generate(designA, _calculationService.Calculate(designA));
        var modelB = _generator.Generate(designB, _calculationService.Calculate(designB));

        // 1. Scale changes
        Assert.NotEqual(modelA.Scale, modelB.Scale);

        // 2. Span dimension label changes
        Assert.Contains(modelA.Dimensions, d => d.Label == "18,000 mm");
        Assert.Contains(modelB.Dimensions, d => d.Label == "30,000 mm");

        // 3. Lift height dimension label changes
        Assert.Contains(modelA.Dimensions, d => d.Label == "6,000 mm");
        Assert.Contains(modelB.Dimensions, d => d.Label == "10,000 mm");

        // 4. Building height dimension label changes
        Assert.Contains(modelA.Dimensions, d => d.Label == "8,000 mm");
        Assert.Contains(modelB.Dimensions, d => d.Label == "12,000 mm");

        // 5. Hoist position dimension label changes
        Assert.Contains(modelA.Dimensions, d => d.Label == "Hoist 9,000 mm");
        Assert.Contains(modelB.Dimensions, d => d.Label == "Hoist 15,000 mm");

        // 6. Technical data differences
        Assert.NotEqual(modelA.TechnicalData.CraneWeight, modelB.TechnicalData.CraneWeight);
        Assert.NotEqual(modelA.TechnicalData.TrolleyWeight, modelB.TechnicalData.TrolleyWeight);
        Assert.NotEqual(modelA.TechnicalData.TotalSupplyPower, modelB.TechnicalData.TotalSupplyPower);

        // 7. SVG rendering outputs differ
        var svgA = _svgRenderer.Render(modelA);
        var svgB = _svgRenderer.Render(modelB);
        Assert.NotEqual(svgA, svgB);
        Assert.Contains("18,000 mm", svgA);
        Assert.Contains("30,000 mm", svgB);

        // 8. PDF rendering outputs differ
        var pdfA = _pdfRenderer.Render(modelA);
        var pdfB = _pdfRenderer.Render(modelB);
        Assert.NotEqual(pdfA.Length, pdfB.Length);
        Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(pdfA, 0, 5));
        Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(pdfB, 0, 5));
    }
}
