using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingModel
    {
        public string DrawingNumber { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Scale { get; set; } = "NTS";

        public List<DrawingView> Views { get; set; } = [];

        public List<DrawingElement> Elements { get; set; } = [];

        public List<DrawingDimension> Dimensions { get; set; } = [];

        public DrawingTechnicalData TechnicalData { get; set; }
            = new();
    }
}
