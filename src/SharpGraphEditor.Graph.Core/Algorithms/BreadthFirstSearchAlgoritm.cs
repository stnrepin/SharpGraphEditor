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

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            if (graph.Vertices.Count() == 0)
            {
                p.Output.WriteLine("Graph is Empty.");
                return;
            }

            var bfs = new Helpers.BreadthFirstSearch(graph)
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
            
            bfs.Run(graph.Vertices.First());
        }
    }
}
