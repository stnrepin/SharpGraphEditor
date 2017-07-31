using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class TestAlgorithm : IAlgorithm
    {
        public String Description { get; } = "Test algorithm";

        public String Name { get; } = "Test algorithm";

        public AlgorithmResult Run(IGraph graph, IAlgorithmHost host)
        {
            host.ShowComment("Please, select vertex.");
            var selectedVertex = host.GetSelectedVertex();
            graph.ChangeColor(selectedVertex, VertexColor.Red);
            return new AlgorithmResult(true, false);
        }
    }
}
