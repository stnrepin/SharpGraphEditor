namespace SharpGraphEditor.Graph.Core.Elements
{
    public interface IEdge : IGraphElement
    {
        IVertex Source { get; set; }

        IVertex Target { get; set; }

        bool IsDirected { get; set; }
    }
}
