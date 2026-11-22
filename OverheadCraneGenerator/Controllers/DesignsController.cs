using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OverheadCraneGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignsController : ControllerBase
    {
        private readonly IDesignService _service;

        public DesignsController(IDesignService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DesignResponse>>> GetAll()
        {
            var designs = await _service.GetAllAsync();

            return Ok(designs);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DesignResponse>> GetById(int id)
        {
            var design = await _service.GetByIdAsync(id);

            if (design == null)
            {
                return NotFound();
            }

            return Ok(design);
        }

        [HttpPost]
        public async Task<ActionResult<DesignResponse>> Create(
            CreateDesignRequest request)
        {
            var design = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = design.Id },
                design);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateDesignRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
