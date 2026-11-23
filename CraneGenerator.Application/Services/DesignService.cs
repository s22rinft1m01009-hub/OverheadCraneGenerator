using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using CraneGenerator.Infrastructure.Repositories;
using CraneGenerator.Application.Validation;
using CraneGenerator.Domain.Enums;

namespace CraneGenerator.Application.Services
{
    public class DesignService : IDesignService
    {
        private readonly IDesignRepository _repository;
        public DesignService(IDesignRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DesignResponse>> GetAllAsync()
        {
            var designs = await _repository.GetAllAsync();

            return designs.Select(MapToResponse).ToList();
        }

        public async Task<DesignResponse?> GetByIdAsync(int id)
        {
            var design = await _repository.GetByIdAsync(id);

            return design == null
                ? null
                : MapToResponse(design);
        }

        public async Task<DesignResponse> CreateAsync(
            CreateDesignRequest request)
        {
            var normalizedCraneType = DesignValidator.ValidateAndNormalize(request);

            var now = DateTime.UtcNow;

            var design = new CraneDesign
            {
                DesignName = request.DesignName.Trim(),
                CraneType = normalizedCraneType,
                CapacityTonnes = request.CapacityTonnes,
                SpanMm = request.SpanMm,
                LiftHeightMm = request.LiftHeightMm,
                RunwayWidthMm = request.RunwayWidthMm,
                HoistPositionMm = request.HoistPositionMm,
                BuildingHeightMm = request.BuildingHeightMm,
                CreatedAt = now,
                UpdatedAt = now
            };

            var created = await _repository.AddAsync(design);

            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateDesignRequest request)
        {
            var normalizedCraneType = DesignValidator.ValidateAndNormalize(request);

            var design = await _repository.GetByIdAsync(id);

            if (design == null)
            {
                return false;
            }

            design.DesignName = request.DesignName.Trim();
            design.CraneType = normalizedCraneType;
            design.CapacityTonnes = request.CapacityTonnes;
            design.SpanMm = request.SpanMm;
            design.LiftHeightMm = request.LiftHeightMm;
            design.RunwayWidthMm = request.RunwayWidthMm;
            design.HoistPositionMm = request.HoistPositionMm;
            design.BuildingHeightMm = request.BuildingHeightMm;
            design.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(design);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var design = await _repository.GetByIdAsync(id);

            if (design == null)
            {
                return false;
            }

            await _repository.DeleteAsync(design);

            return true;
        }

        private static DesignResponse MapToResponse(CraneDesign design)
        {
            return new DesignResponse
            {
                Id = design.Id,
                DesignName = design.DesignName,
                CraneType = CraneTypeNormalizer.Normalize(design.CraneType),
                CapacityTonnes = design.CapacityTonnes,
                SpanMm = design.SpanMm,
                LiftHeightMm = design.LiftHeightMm,
                RunwayWidthMm = design.RunwayWidthMm,
                HoistPositionMm = design.HoistPositionMm,
                BuildingHeightMm = design.BuildingHeightMm,
                CreatedAt = design.CreatedAt,
                UpdatedAt = design.UpdatedAt
            };
        }
    }
}
