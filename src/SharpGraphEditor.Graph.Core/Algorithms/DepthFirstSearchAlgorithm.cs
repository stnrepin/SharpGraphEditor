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

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            if (graph.Vertices.Count() == 0)
            {
                return;
            }

            var dfs = new Helpers.DepthFirstSearch(graph)
            {
                ProcessEdge = (v1, v2) =>
                {
                    graph.ChangeColor(v2, VertexColor.Gray);
                    System.Threading.Thread.Sleep(500);
                },
                ProcessVertexLate = (v) =>
                {
                    graph.ChangeColor(v, VertexColor.Black);
                    System.Threading.Thread.Sleep(500);
                }
            };
            dfs.Run(graph.Vertices.First());
        }
    }
}
