using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingRectangle : DrawingElement
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public string Fill { get; set; } = "none";
    }
}
