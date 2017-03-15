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

            var dfsResult = new Dictionary<int, List<int>>();
            var dfs = new Helpers.DepthFirstSearch(graph)
            {
                ProcessEdge = (parent, child) =>
                {
                    if (dfsResult.ContainsKey(parent.Index))
                    {
                        dfsResult[parent.Index].Add(child.Index);
                    }
                    else
                    {
                        dfsResult.Add(parent.Index, new List<int>() { child.Index });
                    }
                }
            };
            dfs.Run(graph.Vertices.First());

            BuildSearchTree(graph, dfsResult);
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
                    graph.AddEdge(parentVertex, childVertex, isDirected: true, makeNotDirectedIfreversedExisted: false);
                }
            }
        }
    }
}
