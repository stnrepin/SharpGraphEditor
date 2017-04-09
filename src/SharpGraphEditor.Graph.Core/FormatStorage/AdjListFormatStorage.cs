using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpGraphEditor.Graph.Core.Extentions;
using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class AdjListFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            var lines = ReadAllLines(stream);

            var list = new List<IEnumerable<int>>();
            foreach (var line in lines)
            {
                var verticesIndexes = line.Split(' ').Select(x => ParseStringTo<int>(x));
                list.Add(verticesIndexes);
            }

            foreach (var column in list)
            {
                var parentVertexIndex = column.First();
                var parentVertex = graph.AddVertex(parentVertexIndex);

                var vertices = column.Skip(1);
                foreach (var vertex in vertices)
                {
                    var targetVertex = graph.AddVertex(vertex);
                    graph.AddEdge(parentVertex, targetVertex, true);
                }
            }
        }

        public override void Save(TextWriter stream, IGraph graph)
        {
            var adjList = graph.ToAdjList();
            adjList.Select(x => x.Key.Index + " " + String.Join(" ", x.Value.Select(y => y.Index.ToString())))
                   .ForEach(y => stream.WriteLine(y));
        }
    }
}
