using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.CraneModels;

public interface ICraneGeometryGenerator
{
    DrawingModel Generate(DesignResponse design);
}