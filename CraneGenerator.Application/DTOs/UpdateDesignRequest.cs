using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Application.DTOs
{
    public class UpdateDesignRequest
    {
        public string DesignName { get; set; } = string.Empty;

        public string CraneType { get; set; } = string.Empty;

        public double CapacityTonnes { get; set; }

        public double SpanMm { get; set; }

        public double LiftHeightMm { get; set; }

        public double RunwayWidthMm { get; set; }

        public double HoistPositionMm { get; set; }

        public double BuildingHeightMm { get; set; }
    }
}
