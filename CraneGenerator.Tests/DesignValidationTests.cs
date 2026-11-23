using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Validation;
using Xunit;

namespace CraneGenerator.Tests;

public sealed class DesignValidationTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void CapacityMustBePositive(double capacity)
    {
        var request = ValidRequest();
        request.CapacityTonnes = capacity;

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void SpanMustBePositive(double span)
    {
        var request = ValidRequest();
        request.SpanMm = span;

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void LiftHeightMustBePositive(double liftHeight)
    {
        var request = ValidRequest();
        request.LiftHeightMm = liftHeight;

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(25001)]
    public void HoistPositionMustBeInsideSpan(double position)
    {
        var request = ValidRequest();
        request.HoistPositionMm = position;

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Fact]
    public void DesignNameMustNotBeEmpty()
    {
        var request = ValidRequest();
        request.DesignName = " ";

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Fact]
    public void UnsupportedCraneTypeIsRejected()
    {
        var request = ValidRequest();
        request.CraneType = "JIB";

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    [Fact]
    public void GantryAllowsZeroBuildingHeightAndRunwayWidth()
    {
        var request = ValidRequest();
        request.CraneType = "GANTRY";
        request.BuildingHeightMm = 0;
        request.RunwayWidthMm = 0;

        Assert.Equal("GANTRY", DesignValidator.ValidateAndNormalize(request));
    }

    [Fact]
    public void EotRequiresPositiveRunwayWidthAndBuildingHeight()
    {
        var request = ValidRequest();
        request.RunwayWidthMm = 0;
        request.BuildingHeightMm = 0;

        Assert.Throws<DesignValidationException>(() => DesignValidator.ValidateAndNormalize(request));
    }

    private static CreateDesignRequest ValidRequest() => new()
    {
        DesignName = "Test Crane",
        CraneType = "SINGLE_GIRDER_EOT",
        CapacityTonnes = 10,
        SpanMm = 25000,
        LiftHeightMm = 8000,
        RunwayWidthMm = 12500,
        HoistPositionMm = 12500,
        BuildingHeightMm = 9000
    };
}