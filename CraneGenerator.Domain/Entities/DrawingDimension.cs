using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingDimension 
    {
        public double X1 { get; set; }

        public double Y1 { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        public string Label { get; set; } = string.Empty;

        public double Offset { get; set; } = 20;
    }
}
