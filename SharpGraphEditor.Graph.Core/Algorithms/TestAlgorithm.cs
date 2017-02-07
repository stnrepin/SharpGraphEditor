using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class TestAlgorithm : IAlgorithm
    {
        public String Description { get; } = "Test algorithm";

        public String Name { get; } = "Test algorithm";

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            graph.AddVertex(200, 200);
        }
    }
}
