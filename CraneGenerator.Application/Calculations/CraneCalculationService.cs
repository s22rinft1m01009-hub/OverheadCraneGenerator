using System;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Calculations;

public sealed class CraneCalculationService : ICraneCalculationService
{
    public CraneCalculationResult Calculate(DesignResponse design)
    {
        ArgumentNullException.ThrowIfNull(design);

        var span = Math.Max(0, design.SpanMm);
        var hoistPos = Math.Max(0, design.HoistPositionMm);
        var hoistPositionRatio = span <= 0 ? 0 : Math.Clamp(hoistPos / span, 0, 1);

        // Standardized drawing dimensions
        var spanDrawingWidth = 580d;
        var buildingHeightDrawing = Math.Clamp(design.BuildingHeightMm / 100000d * 170d, 90d, 170d);
        var liftRopeLength = Math.Clamp(design.LiftHeightMm / 100d, 35d, 120d);
        var runwayDrawingDepth = Math.Clamp(design.RunwayWidthMm / 1000d, 25d, 70d);

        var scaleDenominator = span <= 0 ? 1 : Math.Max(1, Math.Ceiling(span / spanDrawingWidth));
        var drawingScale = $"1:{scaleDenominator:0}";

        var hoistDrawingX = 100 + hoistPositionRatio * spanDrawingWidth;
        var craneElevation = 60 + buildingHeightDrawing * 0.4;

        // Engineering estimates (clearly identified as estimates/prototype rules)
        var estCraneWeight = Math.Round(design.CapacityTonnes * 0.35 + (span / 1000d) * 0.22 + 2.5, 1);
        var estTrolleyWeight = Math.Round(design.CapacityTonnes * 0.08 + 0.35, 2);
        var estPower = Math.Round(design.CapacityTonnes * 0.95 + 4.8, 1);

        return new CraneCalculationResult
        {
            SpanMm = design.SpanMm,
            LiftHeightMm = design.LiftHeightMm,
            RunwayWidthMm = design.RunwayWidthMm,
            BuildingHeightMm = design.BuildingHeightMm,
            HoistPositionMm = design.HoistPositionMm,
            CapacityTonnes = design.CapacityTonnes,

            HoistPositionRatio = hoistPositionRatio,
            DrawingScaleDenominator = scaleDenominator,
            DrawingScale = drawingScale,
            SpanDrawingWidth = spanDrawingWidth,
            DrawingSpan = spanDrawingWidth,
            DrawingHeight = 200,
            BuildingHeightDrawing = buildingHeightDrawing,
            CraneElevation = craneElevation,
            LiftRopeLength = liftRopeLength,
            RunwayDrawingDepth = runwayDrawingDepth,
            HoistDrawingX = hoistDrawingX,

            SideClearanceLeftMm = 235,
            SideClearanceRightMm = 235,
            OverheadClearanceMm = 200,
            HookApproachLeftMm = 920,
            HookApproachRightMm = 1150,

            HoistingSpeed = "6/1 m/min 2-SPEED",
            TraversingSpeed = "0-20 m/min STEPLESS",
            TravellingSpeed = "0-32 m/min STEPLESS",
            EstimatedCraneWeightTonnes = estCraneWeight,
            EstimatedTrolleyWeightTonnes = estTrolleyWeight,
            EstimatedTotalPowerKw = estPower,
            PowerSupply = "380V / 48V / 50Hz",
            CraneGroup = "FEM A5 (ISO M5)",
            HoistGroup = "M5 (FEM 2m)",
            BridgeTravelGroup = "M5 (FEM 2m)",
            ToleranceStandard = "ACC. TO ISO/FEM STANDARD",
            BridgePaint = "E120 / RAL1028 (MELON YELLOW)"
        };
    }

    public static DrawingTechnicalData BuildTechnicalData(DesignResponse design, CraneCalculationResult calc, string drawingNumber)
    {
        var data = new DrawingTechnicalData
        {
            DesignName = design.DesignName,
            CraneType = design.CraneType,
            DrawingNumber = drawingNumber,
            Revision = "A",
            Note = "CONCEPTUAL DRAWING - NOT FOR FABRICATION",
            CapacityTonnes = design.CapacityTonnes,
            SpanMm = design.SpanMm,
            LiftHeightMm = design.LiftHeightMm,
            RunwayWidthMm = design.RunwayWidthMm,
            HoistPositionMm = design.HoistPositionMm,
            BuildingHeightMm = design.BuildingHeightMm,

            HoistingSpeed = calc.HoistingSpeed,
            TraversingSpeed = calc.TraversingSpeed,
            TravellingSpeed = calc.TravellingSpeed,
            TrolleyWeight = $"{calc.EstimatedTrolleyWeightTonnes:0.00} t",
            CraneWeight = $"{calc.EstimatedCraneWeightTonnes:0.0} t",
            PowerSupply = calc.PowerSupply,
            TotalSupplyPower = $"{calc.EstimatedTotalPowerKw:0.0} kW",
            CraneGroup = calc.CraneGroup,
            HoistGroup = calc.HoistGroup,
            BridgeTravelGroup = calc.BridgeTravelGroup,
            Tolerance = calc.ToleranceStandard,
            BridgePaint = calc.BridgePaint,

            Specifications =
            [
                new("Design Name", design.DesignName, SpecificationSource.UserInput),
                new("Crane Type", design.CraneType, SpecificationSource.UserInput),
                new("Rated Capacity", $"{design.CapacityTonnes:0.0} t", SpecificationSource.UserInput),
                new("Span (S)", $"{design.SpanMm:N0} mm", SpecificationSource.UserInput),
                new("Height of Lift (H)", $"{design.LiftHeightMm:N0} mm", SpecificationSource.UserInput),
                new("Runway Width (B)", $"{design.RunwayWidthMm:N0} mm", SpecificationSource.UserInput),
                new("Hoist Position", $"{design.HoistPositionMm:N0} mm", SpecificationSource.UserInput),
                new("Building Height", $"{design.BuildingHeightMm:N0} mm", SpecificationSource.UserInput),
                new("Hoisting Speed", calc.HoistingSpeed, SpecificationSource.Estimate),
                new("Traversing Speed", calc.TraversingSpeed, SpecificationSource.Estimate),
                new("Travelling Speed", calc.TravellingSpeed, SpecificationSource.Estimate),
                new("Trolley Weight", $"{calc.EstimatedTrolleyWeightTonnes:0.00} t", SpecificationSource.Estimate),
                new("Total Crane Weight", $"{calc.EstimatedCraneWeightTonnes:0.0} t", SpecificationSource.Estimate),
                new("Power Supply", calc.PowerSupply, SpecificationSource.PrototypeRule),
                new("Total Supply Power", $"{calc.EstimatedTotalPowerKw:0.0} kW", SpecificationSource.Estimate),
                new("Crane Group", calc.CraneGroup, SpecificationSource.PrototypeRule),
                new("Hoist Group", calc.HoistGroup, SpecificationSource.PrototypeRule),
                new("Bridge Travel Group", calc.BridgeTravelGroup, SpecificationSource.PrototypeRule),
                new("Tolerances", calc.ToleranceStandard, SpecificationSource.Reference),
                new("Bridge Paint", calc.BridgePaint, SpecificationSource.Reference)
            ]
        };

        return data;
    }
}
