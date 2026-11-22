using CraneGenerator.Domain.Entities;
using CraneGenerator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Infrastructure.Repositories
{
    public class DesignRepository : IDesignRepository
    {
        private readonly CraneDbContext _context;

        public DesignRepository(CraneDbContext context)
        {
            _context = context;
        }

        public async Task<List<CraneDesign>> GetAllAsync()
        {
            return await _context.CraneDesigns
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<CraneDesign?> GetByIdAsync(int id)
        {
            return await _context.CraneDesigns
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CraneDesign> AddAsync(CraneDesign design)
        {
            await _context.CraneDesigns.AddAsync(design);
            await _context.SaveChangesAsync();

            return design;
        }

        public async Task UpdateAsync(CraneDesign design)
        {
            _context.CraneDesigns.Update(design);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CraneDesign design)
        {
            _context.CraneDesigns.Remove(design);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.CraneDesigns
                .AnyAsync(x => x.Id == id);
        }
    }
}
