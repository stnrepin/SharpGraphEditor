using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class RandomLayouterAlgorithm : IAlgorithm
    {
        private static Random _random = new Random();

        public string Name { get; } = "Random layouter";
        public string Description { get; } = "Randomize vertices position";

        public AlgorithmResult Run(IGraph graph, IAlgorithmHost host)
        {
            foreach (var v in graph.Vertices)
            {
                v.X = _random.Next((int)host.MinElementX, (int)(host.MaxElementX - host.MinElementX));
                v.Y = _random.Next((int)host.MinElementY, (int)(host.MaxElementY - host.MinElementY));
            }
            return new AlgorithmResult(true, false);
        }
    }
}
