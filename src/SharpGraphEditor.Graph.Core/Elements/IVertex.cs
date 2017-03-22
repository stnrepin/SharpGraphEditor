﻿namespace SharpGraphEditor.Graph.Core.Elements
{
    public enum VertexColor
    {
        White,
        Gray,
        Black
    }

    public interface IVertex : IGraphElement
    {
        int Index { get; }

        string Title { get; set; }

        bool HasPosition { get; set; }

        VertexColor Color { get; set; }
    }
}
