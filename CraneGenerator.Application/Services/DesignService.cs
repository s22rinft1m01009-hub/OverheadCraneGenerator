using AutoMapper;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using CraneGenerator.Infrastructure.Repositories;

namespace CraneGenerator.Application.Services
{
    public class DesignService : IDesignService
    {
        private readonly IDesignRepository _repository;
        private readonly IMapper _mapper;
        public DesignService(IDesignRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            Validate(request);

            var now = DateTime.UtcNow;

            var design = new CraneDesign
            {
                DesignName = request.DesignName.Trim(),
                CraneType = request.CraneType.Trim(),
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
            Validate(request);

            var design = await _repository.GetByIdAsync(id);

            if (design == null)
            {
                return false;
            }

            design.DesignName = request.DesignName.Trim();
            design.CraneType = request.CraneType.Trim();
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

        private static void Validate(CreateDesignRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DesignName))
                throw new ArgumentException("Design name is required.");

            if (request.CapacityTonnes <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            if (request.SpanMm <= 0)
                throw new ArgumentException(
                    "Span must be greater than zero.");

            if (request.LiftHeightMm <= 0)
                throw new ArgumentException(
                    "Lift height must be greater than zero.");

            if (request.RunwayWidthMm <= 0)
                throw new ArgumentException(
                    "Runway width must be greater than zero.");

            if (request.BuildingHeightMm <= 0)
                throw new ArgumentException(
                    "Building height must be greater than zero.");

            if (request.HoistPositionMm < 0 ||
                request.HoistPositionMm > request.SpanMm)
            {
                throw new ArgumentException(
                    "Hoist position must be between 0 and the span.");
            }
        }

        private static void Validate(UpdateDesignRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.DesignName))
                throw new ArgumentException("Design name is required.");

            if (request.CapacityTonnes <= 0)
                throw new ArgumentException(
                    "Capacity must be greater than zero.");

            if (request.SpanMm <= 0)
                throw new ArgumentException(
                    "Span must be greater than zero.");

            if (request.LiftHeightMm <= 0)
                throw new ArgumentException(
                    "Lift height must be greater than zero.");

            if (request.RunwayWidthMm <= 0)
                throw new ArgumentException(
                    "Runway width must be greater than zero.");

            if (request.BuildingHeightMm <= 0)
                throw new ArgumentException(
                    "Building height must be greater than zero.");

            if (request.HoistPositionMm < 0 ||
                request.HoistPositionMm > request.SpanMm)
            {
                throw new ArgumentException(
                    "Hoist position must be between 0 and the span.");
            }
        }

        private static DesignResponse MapToResponse(CraneDesign design)
        {
            return new DesignResponse
            {
                Id = design.Id,
                DesignName = design.DesignName,
                CraneType = design.CraneType,
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
