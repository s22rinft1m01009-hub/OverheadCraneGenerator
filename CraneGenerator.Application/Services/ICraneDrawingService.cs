using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Application.Services
{
    public interface ICraneDrawingService 
    {
        DrawingModel Generate(DesignResponse design);
    }
}
