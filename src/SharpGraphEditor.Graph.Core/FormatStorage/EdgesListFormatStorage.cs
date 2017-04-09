using System;
using System.IO;
using System.Linq;

using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class EdgesListFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            var lines = ReadAllLines(stream).ToArray();
            foreach (var line in lines)
            {
                var edges = line.Split(' ').Select(x => ParseStringTo<int>(x)).ToArray();
                if (edges.Length != 2)
                {
                    throw new ArgumentOutOfRangeException("Bad format of edges list file");
                }

                var v1 = graph.AddVertex(edges[0]);
                var v2 = graph.AddVertex(edges[1]);
                graph.AddEdge(v1, v2, true);
            }
        }

        public override void Save(TextWriter stream, IGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                stream.WriteLine(edge.Source.Index.ToString() + " " + edge.Target.Index.ToString());
            }
        }
    }
}
