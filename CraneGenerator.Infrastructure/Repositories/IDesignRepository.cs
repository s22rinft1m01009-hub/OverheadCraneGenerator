using CraneGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Infrastructure.Repositories
{
    public interface IDesignRepository
    {
        Task<List<CraneDesign>> GetAllAsync();

        Task<CraneDesign?> GetByIdAsync(int id);

        Task<CraneDesign> AddAsync(CraneDesign design);

        Task UpdateAsync(CraneDesign design);

        Task DeleteAsync(CraneDesign design);

        Task<bool> ExistsAsync(int id);
    }
}
