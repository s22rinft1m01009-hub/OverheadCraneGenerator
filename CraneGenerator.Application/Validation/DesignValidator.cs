using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Enums;

namespace CraneGenerator.Application.Validation;

public static class DesignValidator
{
    public static string ValidateAndNormalize(CreateDesignRequest request)
    {
        return ValidateAndNormalize(
            request.DesignName,
            request.CraneType,
            request.CapacityTonnes,
            request.SpanMm,
            request.LiftHeightMm,
            request.RunwayWidthMm,
            request.HoistPositionMm,
            request.BuildingHeightMm);
    }

    public static string ValidateAndNormalize(UpdateDesignRequest request)
    {
        return ValidateAndNormalize(
            request.DesignName,
            request.CraneType,
            request.CapacityTonnes,
            request.SpanMm,
            request.LiftHeightMm,
            request.RunwayWidthMm,
            request.HoistPositionMm,
            request.BuildingHeightMm);
    }

    private static string ValidateAndNormalize(
        string? designName,
        string? craneType,
        double capacityTonnes,
        double spanMm,
        double liftHeightMm,
        double runwayWidthMm,
        double hoistPositionMm,
        double buildingHeightMm)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(designName))
            errors.Add("DesignName is required.");
        else if (designName.Trim().Length > 200)
            errors.Add("DesignName must be 200 characters or fewer.");

        string normalizedCraneType = string.Empty;
        try
        {
            normalizedCraneType = CraneTypeNormalizer.Normalize(craneType);
        }
        catch (ArgumentException)
        {
            errors.Add("CraneType must be SINGLE_GIRDER_EOT, DOUBLE_GIRDER_EOT, or GANTRY.");
        }

        RequireFinitePositive(capacityTonnes, "CapacityTonnes", errors);
        RequireFinitePositive(spanMm, "SpanMm", errors);
        RequireFinitePositive(liftHeightMm, "LiftHeightMm", errors);
        RequireFiniteNonNegative(runwayWidthMm, "RunwayWidthMm", errors);
        RequireFiniteNonNegative(hoistPositionMm, "HoistPositionMm", errors);
        RequireFinitePositive(buildingHeightMm, "BuildingHeightMm", errors);

        if (spanMm > 0 && hoistPositionMm > spanMm)
            errors.Add("HoistPositionMm must be less than or equal to SpanMm.");

        if (normalizedCraneType is CraneTypeNormalizer.SingleGirderEot or CraneTypeNormalizer.DoubleGirderEot
            && runwayWidthMm <= 0)
        {
            errors.Add("RunwayWidthMm must be greater than zero for EOT cranes.");
        }

        if (errors.Count > 0)
            throw new DesignValidationException(errors);

        return normalizedCraneType;
    }

    private static void RequireFinitePositive(double value, string name, ICollection<string> errors)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            errors.Add($"{name} must be a finite value greater than zero.");
    }

    private static void RequireFiniteNonNegative(double value, string name, ICollection<string> errors)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
            errors.Add($"{name} must be a finite value greater than or equal to zero.");
    }
}
