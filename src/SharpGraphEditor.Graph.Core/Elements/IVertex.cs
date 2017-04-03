namespace SharpGraphEditor.Graph.Core.Elements
{
    public enum VertexColor
    {
        White,
        Gray,
        Black,
        Green,
        Red,
        Blue
    }

    public interface IVertex : IGraphElement
    {
        string Name { get; set; }

        int Index { get; }

        string Title { get; set; }

        bool HasPosition { get; set; }

        VertexColor Color { get; set; }
    }
}
