using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms.Helpers
{
    class DepthFirstSearch
    {
        private IGraph _graph;
        private IVertex _parent;
        private Dictionary<IVertex, IEnumerable<IVertex>> _adjList;

        private bool[] _isDiscovered;
        private bool[] _isProcessed;
        private int[] _entryTime;
        private int _verticesCount;
        private int _time;
        private bool _isFinished;

        public Action<IVertex> ProcessVertexEarly { get; set; }
        public Action<IVertex> ProcessVertexLate { get; set; }
        public Action<IVertex, IVertex> ProcessEdge { get; set; }

        public DepthFirstSearch(IGraph graph)
        {
            _graph = graph;
            _adjList = graph.ToAdjList();
            _verticesCount = graph.Vertices.Count();

            _isDiscovered = new bool[_verticesCount];
            _isProcessed = new bool[_verticesCount];
            _entryTime = new int[_verticesCount];
        }

        public void Cancel()
        {
            _isFinished = true;
        }

        public void Run(IVertex vertex)
        {
            if (_verticesCount == 0 || _isFinished)
            {
                return;
            }

            ProcessVertexEarly?.Invoke(vertex);

            _time++;
            _entryTime[vertex.Index - 1] = _time;
            _isDiscovered[vertex.Index - 1] = true;

            foreach (var adjVertex in _adjList[vertex])
            {
                if (!_isDiscovered[adjVertex.Index - 1])
                {
                    _parent = vertex;
                    ProcessEdge?.Invoke(vertex, adjVertex);
                    Run(adjVertex);
                }
                else if (!_isProcessed[adjVertex.Index - 1] || _graph.IsDirected)
                {
                    ProcessEdge?.Invoke(vertex, adjVertex);
                }
                if (_isFinished)
                {
                    return;
                }
            }

            ProcessVertexLate?.Invoke(vertex);
            _time++;
            // write exit time
            _isProcessed[vertex.Index - 1] = true;
        }
    }
}
