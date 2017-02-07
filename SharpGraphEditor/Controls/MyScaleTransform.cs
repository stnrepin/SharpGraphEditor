using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Controls
{
    public class MyScaleTransform : IGraphElement
    {
        public bool IsAdding { get; set; } = false;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
    }
}
