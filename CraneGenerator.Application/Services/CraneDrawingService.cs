using CraneGenerator.Application.CraneModels;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Services;

public sealed class CraneDrawingService : ICraneDrawingService
{
    private readonly CraneDrawingFactory _factory;

    public CraneDrawingService(CraneDrawingFactory factory)
    {
        _factory = factory;
    }

    public DrawingModel Generate(DesignResponse design)
    {
        return _factory.Generate(design);
    }
}