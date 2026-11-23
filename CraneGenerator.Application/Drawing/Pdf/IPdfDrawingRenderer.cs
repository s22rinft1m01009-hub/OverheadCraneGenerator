using CraneGenerator.Domain.Entities;

namespace CraneGenerator.Application.Drawing.Pdf;

public interface IPdfDrawingRenderer
{
    byte[] Render(DrawingModel model);
}
