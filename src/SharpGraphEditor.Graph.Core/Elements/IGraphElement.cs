namespace SharpGraphEditor.Graph.Core.Elements
{
    public interface IGraphElement
    {
        double X { get; set; }

        double Y { get; set; }

        bool IsAdding { get; set; }
    }
}
