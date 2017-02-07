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

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            foreach (var v in graph.Vertices)
            {
                v.X = _random.Next((int)p.MinElementX, (int)(p.MaxElementX - p.MinElementX));
                v.Y = _random.Next((int)p.MinElementY, (int)(p.MaxElementY - p.MinElementY));
            }
        }
    }
}
