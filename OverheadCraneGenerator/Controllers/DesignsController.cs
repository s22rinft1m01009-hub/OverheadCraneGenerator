using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OverheadCraneGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignsController : AppBaseController
    {
        private readonly IDesignService _service;

        public DesignsController(IDesignService service)
        {
            _service = service;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var designs = await _service.GetAllAsync();

            return Success(designs, "Designs retrieved successfully");
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var design = await _service.GetByIdAsync(id);

            if (design == null)
            {
                return NotFound();
            }

            return Success(design, "Data Reterive succesfully");
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateDesignRequest request)
        {
            var design = await _service.CreateAsync(request);

            return Success(design, "Data Add succesfully");
            //return CreatedAtAction(
            //    nameof(GetById),
            //    new { id = design.Id },
            //    design);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDesignRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound("Id is dose not Exist in Table");
            }
            return Success(updated, "Data Add succesfully");
            
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound("Id is dose not Exist in Table");
            }

            return Success("", "Data Delete succesfully");
        }
    }
}
