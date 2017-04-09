using System.IO;
using System.Linq;

using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class AdjMatrixFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            var lines = ReadAllLines(stream).Select(x => x.Split(' ')).ToList();

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

        public override void Save(TextWriter stream, IGraph graph)
        {
            var verticesCount = graph.Vertices.Count();
            var matrix = new int[verticesCount, verticesCount];

            foreach (var pair in graph.ToAdjList())
            {
                var index = pair.Key.Index;
                foreach (var adjVertex in pair.Value)
                {
                    matrix[index - 1, adjVertex.Index - 1] = 1;
                }
            }

            PrintMatrix(stream, matrix, verticesCount, verticesCount);
        }
    }
}
