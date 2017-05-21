using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; }
        string Description { get; }

        AlgorithmResult Run(IGraph graph, IAlgorithmHost host);
    }
}
