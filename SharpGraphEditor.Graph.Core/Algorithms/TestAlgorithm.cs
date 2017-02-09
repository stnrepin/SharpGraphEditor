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

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            throw new ArgumentException("Some error!!!");
        }
    }
}
