using System;
using System.Collections.Generic;
using System.Text;

namespace CraneGenerator.Domain.Entities
{
    public class DrawingView
    {
        public string Name { get; set; } = string.Empty;

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
