using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingCircle : DrawingElement
    {
        public double CenterX { get; set; }

        public double CenterY { get; set; }

        public double Radius { get; set; }

        public string Fill { get; set; } = "none";
    }
}
