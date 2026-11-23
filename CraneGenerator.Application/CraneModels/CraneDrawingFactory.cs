using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using CraneGenerator.Domain.Enums;

namespace CraneGenerator.Application.CraneModels;

public sealed class CraneDrawingFactory
{
    private readonly IReadOnlyDictionary<CraneType, ICraneGeometryGenerator> _generators;

    public CraneDrawingFactory(
        SingleGirderEotGenerator singleGirderEotGenerator,
        DoubleGirderEotGenerator doubleGirderEotGenerator,
        GantryGenerator gantryGenerator)
    {
        _generators = new Dictionary<CraneType, ICraneGeometryGenerator>
        {
            [CraneType.SingleGirderEot] = singleGirderEotGenerator,
            [CraneType.DoubleGirderEot] = doubleGirderEotGenerator,
            [CraneType.Gantry] = gantryGenerator
        };
    }

    public DrawingModel Generate(DesignResponse design)
    {
        var type = CraneTypeNormalizer.Parse(design.CraneType);
        return _generators[type].Generate(design);
    }
}