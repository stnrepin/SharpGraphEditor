using System.Collections.Generic;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core
{
    public interface IGraph
    {
        IEnumerable<IVertex> Vertices { get; }

        IEnumerable<IEdge> Edges { get; }

        bool IsDirected { get; set; }

        IVertex AddVertex(double x, double y);

        IVertex AddVertex(double x, double y, int index);

        IEdge AddEdge(IVertex source, IVertex target, bool isDirected = false);

        void Remove(IGraphElement element);

        void Clear();

        List<List<IVertex>> ToAdjList();

        IVertex FindVertexByIndex(int index);
    }
}
