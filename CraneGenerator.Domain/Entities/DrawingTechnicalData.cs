using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingTechnicalData 
    {
        public double CapacityTonnes { get; set; }

        public double SpanMm { get; set; }

        public double LiftHeightMm { get; set; }

        public double RunwayWidthMm { get; set; }

        public double HoistPositionMm { get; set; }

        public double BuildingHeightMm { get; set; }
    }
}
