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

            var bfsResult = new List<IVertex>();
            var bfs = new Helpers.BreadthFirstSearch(graph)
            {
                ProcessVertexLate = (v) => bfsResult.Add(v)
            };
            bfs.Run(graph.Vertices.First());

            p.Output.WriteLine($"BFS queue: {String.Join(", ", bfsResult.Select(x => "\"" + x.Title + "\""))}");
        }
    }
}
