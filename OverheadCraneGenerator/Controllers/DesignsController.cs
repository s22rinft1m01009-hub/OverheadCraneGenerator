using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.DTOs;
using CraneGenerator.Application.Services;
using CraneGenerator.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OverheadCraneGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignsController : AppBaseController
    {
        private readonly IDesignService _service;
        private readonly ICraneDrawingService _drawingService;
        private readonly SvgDrawingRenderer _svgDrawingRenderer;
        private readonly IPdfDrawingRenderer _pdfDrawingRenderer;

        public DesignsController(
            IDesignService service,
            ICraneDrawingService drawingService,
            SvgDrawingRenderer svgDrawingRenderer,
            IPdfDrawingRenderer pdfDrawingRenderer)
        {
            _service = service;
            _drawingService = drawingService;
            _svgDrawingRenderer = svgDrawingRenderer;
            _pdfDrawingRenderer = pdfDrawingRenderer;
        }

        [HttpGet]
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
        [HttpPost("{id:int}/generate")]
        public async Task<ActionResult<DrawingModel>> GenerateDrawing(int id)
        {
            var design = await _service.GetByIdAsync(id);

            if (design == null)
            {
                return NotFound();
            }

            var drawing = _drawingService.Generate(design);

            return Ok(drawing);
        }
        [HttpGet("{id:int}/drawing/svg")]
        public async Task<IActionResult> GenerateSvg(int id)
        {
            var design = await _service.GetByIdAsync(id);

            if (design == null)
                return NotFound();

            var drawing = _drawingService.Generate(design);

            var svg = _svgDrawingRenderer.Render(drawing);

            return Content(svg, "image/svg+xml");
        }

        [HttpGet("{id:int}/drawing/pdf")]
        public async Task<IActionResult> GeneratePdf(int id)
        {
            var design = await _service.GetByIdAsync(id);
            if (design == null)
                return NotFound();

            var drawing = _drawingService.Generate(design);
            var pdf = _pdfDrawingRenderer.Render(drawing);
            var safeName = string.Concat(design.DesignName.Select(character =>
                Path.GetInvalidFileNameChars().Contains(character) ? '-' : character));
            var fileName = $"{safeName}-Drawing.pdf";

            return File(pdf, "application/pdf", fileName);
        }
    }
}
