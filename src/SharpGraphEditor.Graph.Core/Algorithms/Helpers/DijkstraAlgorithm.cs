using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms.Helpers
{
    public class DijkstraAlgorithm
    {
        public const int MaxDistance = Int32.MaxValue;

        private IGraph _graph;
        private Dictionary<IVertex, IEnumerable<IVertex>> _adjList;

        public DijkstraAlgorithm(IGraph graph, Dictionary<IVertex, IEnumerable<IVertex>> adjList)
        {
            _graph = graph;
            _adjList = adjList;
        }

        // Use: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
        public Dictionary<IVertex, int> GetShortestPaths(IVertex start)
        {
            var distances = new Dictionary<IVertex, int>();
            var used = new Dictionary<IVertex, bool>();

            foreach (var v in _graph.Vertices)
            {
                distances.Add(v, v == start ? 0 : MaxDistance);
                used.Add(v, false);
            }

            foreach (var i in _graph.Vertices)
            {
                IVertex vertex = null;
                foreach (var j in _graph.Vertices)
                {
                    if (!used[j] && (vertex == null || distances[j] < distances[vertex]))
                    {
                        vertex = j;
                    }
                }
                if (distances[vertex] == MaxDistance)
                {
                    break;
                }
                used[vertex] = true;

                foreach (var neighbor in _adjList[vertex])
                {
                    if (distances[vertex] + 1 < distances[neighbor])
                    {
                        distances[neighbor] = distances[vertex] + 1;
                    }
                }
            }
            return distances;
        }
    }
}
