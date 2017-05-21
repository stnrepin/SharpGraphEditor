using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class DepthFirstSearchAlgorithm : IAlgorithm
    {
        public string Name => "Depth first search";

        public string Description => "Depth first search";

        public AlgorithmResult Run(IGraph graph, IAlgorithmHost host)
        {
            if (graph.Vertices.Count() == 0)
            {
                return new AlgorithmResult(false, false);
            }

            graph.ChangeColor(graph.Vertices.ElementAt(0), VertexColor.Gray);
            var dfs = new Helpers.DepthFirstSearch(graph)
            {
                ProcessEdge = (v1, v2) =>
                {
                    if (v2.Color != VertexColor.Gray)
                    {
                        graph.ChangeColor(v2, VertexColor.Gray);
                    }
                },
                ProcessVertexLate = (v) =>
                {
                    graph.ChangeColor(v, VertexColor.Black);
                }
            };
            dfs.Run(graph.Vertices.First());
            return new AlgorithmResult();
        }
    }
}
