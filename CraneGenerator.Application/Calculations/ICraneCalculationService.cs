using CraneGenerator.Application.DTOs;

namespace CraneGenerator.Application.Calculations;

public interface ICraneCalculationService
{
    CraneCalculationResult Calculate(DesignResponse design);
}
