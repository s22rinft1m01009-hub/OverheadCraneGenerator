using CraneGenerator.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Application.Services
{
    public interface IDesignService
    {
        Task<List<DesignResponse>> GetAllAsync();

        Task<DesignResponse?> GetByIdAsync(int id);

        Task<DesignResponse> CreateAsync(CreateDesignRequest request);

        Task<bool> UpdateAsync(int id, UpdateDesignRequest request);

        Task<bool> DeleteAsync(int id);
    }
}
