using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms.Helpers
{
    public class BreadthFirstSearch
    {
        public Action<IVertex> ProcessEarly;

        public void Run(IGraph graph, IVertex startVertex)
        {
            if (graph.Vertices.Count() == 0)
            {
                return;
            }

            var stack = new Stack<IVertex>();
            stack.Push(startVertex);
        }
    }
}
