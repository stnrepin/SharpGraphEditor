using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class BreadthFirstSearchAlgoritm : IAlgorithm
    {
        public string Name => "Breadth-first search";
        public string Description => "Breadth-first search";

        public AlgorithmResult Run(IGraph graph, IAlgorithmHost host)
        {
            if (graph.Vertices.Count() == 0)
            {
                host.Output.WriteLine("Graph is Empty.");
                return new AlgorithmResult(false, false);
            }

            var selectedVertex = host.GetSelectedVertex();

            var bfs = new Helpers.BreadthFirstSearch(graph)
            {
                ProcessChild = (vParent, vChild) =>
                {
                    graph.ChangeColor(vChild, VertexColor.Gray);
                    host.ShowCommentForLastAction($"Edge connected vertices {vParent.Index} and {vChild.Index} exists. Add vertex {vChild.Index} to queue.");
                    host.AddToTableForLastAction(vChild.Index.ToString());
                },
                ProcessVertexLate = (v) =>
                {
                    graph.ChangeColor(v, VertexColor.Black);
                    host.ShowCommentForLastAction($"Vertex {v.Index} has not unvisited adjacent vertices.");
                    host.ShowComment($"Remove vertex {v.Index} from queue.");
                    host.RemoveRowFromTableForLastAction(v.Index.ToString());
                }
            };

            graph.ChangeColor(selectedVertex, VertexColor.Gray);
            host.ShowCommentForLastAction($"Start BFS from vertex {selectedVertex.Index}.");
            host.AddToTableForLastAction(selectedVertex.Index.ToString());

            bfs.Run(selectedVertex);
            host.ShowComment("BFS has been finished.");

            return new AlgorithmResult();
        }
    }
}
