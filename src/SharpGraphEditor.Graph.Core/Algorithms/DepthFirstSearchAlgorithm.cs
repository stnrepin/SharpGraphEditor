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

            var selectedVertex = host.GetSelectedVertex();

            var dfs = new Helpers.DepthFirstSearch(graph)
            {
                ProcessEdge = (v1, v2) =>
                {
                    if (v2.Color != VertexColor.Gray)
                    {
                        graph.ChangeColor(v2, VertexColor.Gray);
                        host.ShowCommentForLastAction($"Edge connected vertices {v1.Index} and {v2.Index} exists. Add vertex {v2.Index} to stack.");
                    }
                },
                ProcessVertexLate = (v) =>
                {
                    graph.ChangeColor(v, VertexColor.Black);
                    host.ShowCommentForLastAction($"Vertex {v.Index} has not unvisited adjacent vertices.");
                    host.ShowComment("Returns to last vertex.");
                }
            };
            graph.ChangeColor(selectedVertex, VertexColor.Gray);
            host.ShowCommentForLastAction($"Start DFS from vertex {selectedVertex.Index}.");

            dfs.Run(selectedVertex);
            host.ShowComment("DFS has been finished.");

            return new AlgorithmResult();
        }
    }
}
