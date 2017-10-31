using System;
using System.IO;
using System.Linq;

using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class AdjMatrixFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader reader, IGraph graph)
        {
            base.Open(reader, graph);

            var lines = ReadAllLines(reader).Select(x => x.Split(' ')).ToList();

            var count = lines.Count();
            for (var i = 0; i < count; i++)
            {
                var v1 = graph.AddVertex(i + 1);
                for (var j = 0; j < count; j++)
                {
                    if (lines[i][j] != "0")
                    {
                        var v2 = graph.AddVertex(j + 1);
                        graph.AddEdge(v1, v2, true);
                    }
                }
            }
        }

        public override void Save(TextWriter writer, IGraph graph)
        {
            var verticesCount = graph.Vertices.Count();
            var matrix = ToAdjMatrix(graph);
            PrintMatrix(writer, matrix, verticesCount, verticesCount);
        }

        public static int[,] ToAdjMatrix(IGraph graph)
        {
            var verticesCount = graph.Vertices.Count();
            var matrix = new int[verticesCount, verticesCount];

            try
            {
                foreach (var pair in graph.ToAdjList())
                {
                    var index = pair.Key.Index;
                    foreach (var adjVertex in pair.Value)
                    {
                        matrix[index - 1, adjVertex.Index - 1] = 1;
                    }
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new ArgumentException($"graph must contains all vertices between 1 and {graph.Vertices.Max(x => x.Index)}", e);
            }
            return matrix;
        }
    }
}
