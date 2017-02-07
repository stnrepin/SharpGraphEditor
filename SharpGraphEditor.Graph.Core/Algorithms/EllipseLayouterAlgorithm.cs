using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class EllipseLayouterAlgorithm : IAlgorithm
    {
        public string Name { get; } = "Ellipse layouter";
        public string Description { get; } = "Has vertices on the boundary of the ellipse";

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            var width = p.MaxElementX;
            var height = p.MaxElementY;
            var vertices = graph.Vertices;

            var vCount = vertices.Count();
            if (vCount == 0)
            {
                return;
            }
            else if (vCount == 1)
            {
                var v = vertices.First();
                v.X = p.MaxElementX / 2;
                v.Y = p.MaxElementY / 2;
                return;
            }

            var minWidth = p.MinElementX;
            var minHeight = p.MinElementY;

            width -= minWidth * 2.25;
            height -= minHeight * 2.25;

            var centerX = width / 2;
            var centerY = height / 2;

            int i = 0;
            foreach (var v in vertices)
            { 
                v.X = Math.Ceiling(centerX + minWidth + centerX * Math.Cos(Math.PI * i / vCount * 2));
                v.Y = Math.Ceiling(centerY + minHeight + centerY * Math.Sin(Math.PI * i / vCount * 2));
                i++;
            }
        }
    }
}
