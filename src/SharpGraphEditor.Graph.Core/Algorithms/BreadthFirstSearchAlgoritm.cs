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

            var bfsResult = new Dictionary<int, List<int>>();
            var bfs = new Helpers.BreadthFirstSearch(graph)
            {
                ProcessChild = (parent, child) =>
                {
                    if (bfsResult.ContainsKey(parent.Index))
                    {
                        bfsResult[parent.Index].Add(child.Index);
                    }
                    else
                    {
                        bfsResult.Add(parent.Index, new List<int>() { child.Index });
                    }
                }
            };
            
            bfs.Run(graph.Vertices.First());

            BuildSearchTree(graph, bfsResult);
        }

        private void BuildSearchTree(IGraph graph, Dictionary<int, List<int>> bfs)
        {
            graph.Clear();

            foreach (var pair in bfs)
            {
                var parentVertex = graph.AddVertex(pair.Key);
                foreach (var child in pair.Value)
                {
                    var childVertex = graph.AddVertex(child);
                    graph.AddEdge(parentVertex, childVertex);
                }
            }
        }
    }
}
