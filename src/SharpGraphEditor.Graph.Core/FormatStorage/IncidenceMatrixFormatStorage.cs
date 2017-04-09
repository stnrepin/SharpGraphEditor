using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;
using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class IncidenceMatrixFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader reader, IGraph graph)
        {
            base.Open(reader, graph);

            var incMatrix = ReadAllLines(reader).Select(x => x.Split(' ').ToArray())
                                                .ToList();
            var verticesCount = incMatrix.Count;
            if (verticesCount == 0)
            {
                return;
            }

            var edgesCount = incMatrix.First().Length;
            for (int edgeIndex = 0; edgeIndex < edgesCount; edgeIndex++)
            {
                IVertex source = null;
                IVertex target = null;
                bool directed = true;
                for (int vertexIndex = 0; vertexIndex < verticesCount; vertexIndex++)
                {

                    if (incMatrix[vertexIndex][edgeIndex] == "1")
                    {
                        if (source != null)
                        {
                            target = graph.AddVertex(vertexIndex + 1);
                            directed = false;
                        }
                        else
                        {
                            source = graph.AddVertex(vertexIndex + 1);
                        }
                    }
                    else if (incMatrix[vertexIndex][edgeIndex] == "-1")
                    {
                        target = graph.AddVertex(vertexIndex + 1);
                    }
                }
                graph.AddEdge(source, target, directed);
            }
        }

        public override void Save(TextWriter writer, IGraph graph)
        {
            var verticesCount = graph.Vertices.Count();
            var edgesCount = graph.Edges.Count();
            var matrix = new int[verticesCount, edgesCount];
            var edgeNumber = 0;
            foreach (var edge in graph.Edges)
            {
                matrix[edge.Source.Index - 1, edgeNumber] = 1;
                matrix[edge.Target.Index - 1, edgeNumber] = edge.IsDirected ? -1 : 1;
                edgeNumber++;
            }
            PrintMatrix(writer, matrix, verticesCount, edgesCount);
        }
    }
}
