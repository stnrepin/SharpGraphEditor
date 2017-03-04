namespace SharpGraphEditor.Graph.Core.Elements
{
    public interface IVertex : IGraphElement
    {
        int Index { get; }

        string Title { get; set; }

        bool HasPosition { get; set; }
    }
}
