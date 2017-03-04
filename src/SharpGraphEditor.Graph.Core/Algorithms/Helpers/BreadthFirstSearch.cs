using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms.Helpers
{
    public class BreadthFirstSearch
    {
        private IGraph _graph;

        private IVertex[] _parent;
        private bool[] _processed;
        private bool[] _discovered;
        private int _verticesCount;

        public Action<IVertex> ProcessVertexEarly { get; set; }
        public Action<IVertex, IVertex> ProcessEdge { get; set; }
        public Action<IVertex> ProcessVertexLate { get; set; }

        public BreadthFirstSearch(IGraph graph)
        {
            _graph = graph;

            _verticesCount = graph.Vertices.Count();
            _processed = new bool[_verticesCount];
            _discovered = new bool[_verticesCount];
            _parent = new IVertex[_verticesCount];
        }

        public void Run(IVertex startVertex)
        {
            if (_verticesCount == 0)
            {
                return;
            }

            var queue = new Queue<IVertex>();
            queue.Enqueue(startVertex);
            _discovered[startVertex.Index - 1] = true;
            var adjList = _graph.ToAdjList();

            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                ProcessVertexEarly?.Invoke(v);
                _processed[v.Index - 1] = true;
                var p = adjList[v];
                var i = 0;
                var adjVerticesCount = p.Count();
                while (i < adjVerticesCount)
                {
                    var y = p.ElementAt(i);
                    if (!_processed[y.Index - 1] || _graph.IsDirected)
                    {
                        ProcessEdge?.Invoke(v, y);
                    }
                    if (!_discovered[y.Index - 1])
                    {
                        queue.Enqueue(y);
                        _discovered[y.Index - 1] = true;
                        _parent[y.Index - 1] = v;
                    }
                    i++;
                }
                ProcessVertexLate?.Invoke(v);
            }
        }
    }
}
