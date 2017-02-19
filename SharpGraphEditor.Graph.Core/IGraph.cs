using System.Collections.Generic;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core
{
    public interface IGraph
    {
        IEnumerable<IVertex> Vertices { get; }

        IEnumerable<IEdge> Edges { get; }

        bool IsDirected { get; set; }

        IVertex AddVertex(int index);

        IVertex AddVertex(double x, double y);

        IVertex AddVertex(double x, double y, int index);

        IEdge AddEdge(IVertex source, IVertex target, bool isDirected = false, bool directedIfReversedExisting = false);

        void Remove(IGraphElement element);

        void Clear();

        Dictionary<IVertex, IEnumerable<IVertex>> ToAdjList();

        IVertex FindVertexByIndex(int index);
    }
}
