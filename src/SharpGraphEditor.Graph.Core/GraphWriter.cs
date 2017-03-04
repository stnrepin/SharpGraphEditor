using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.Extentions;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core
{
    public static class GraphWriter
    {
        public static void ToIncidenceMatrix(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                using (var stream = new StreamWriter(fileStream))
                {
                    ToIncidenceMatrix(stream, graph);
                }
            }
        }

        public static void ToIncidenceMatrix(TextWriter stream, IGraph graph)
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
            PrintMatrix(stream, matrix, verticesCount, edgesCount);
        }

        public static void ToEdgesList(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                using (var stream = new StreamWriter(fileStream))
                {
                    ToEdgesList(stream, graph);
                }
            }
        }

        public static void ToEdgesList(TextWriter stream, IGraph graph)
        {
            foreach (var edge in graph.Edges)
            {
                stream.WriteLine(edge.Source.Index.ToString() + " " + edge.Target.Index.ToString());
            }
        }

        public static void ToAdjMatrix(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                using (var stream = new StreamWriter(fileStream))
                {
                    ToAdjMatrix(stream, graph);
                }
            }
        }

        public static void ToAdjMatrix(TextWriter stream, IGraph graph)
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

        public static void ToAdjList(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                using (var stream = new StreamWriter(fileStream))
                {
                    ToAdjList(stream, graph);
                }
            }
        }

        public static void ToAdjList(TextWriter stream, IGraph graph)
        {
            var adjList = graph.ToAdjList();
            adjList.Select(x =>  x.Key.Title + " " + String.Join(" ", x.Value.Select(y => y.Index.ToString())))
                   .ForEach(y => stream.WriteLine(y));
        }

        public static void ToGxml(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                fileStream.SetLength(0);
                fileStream.Flush();
                using (var stream = new StreamWriter(fileStream))
                {
                    ToGxml(stream, graph);
                }
            }
        }

        public static void ToGxml(TextWriter stream, IGraph graph)
        {
            var verticesXmlElements = new List<XElement>();
            var edgesXmlElements = new List<XElement>();

            graph.Vertices.ForEach(x =>
            {
                var indexAttr = new XAttribute(XName.Get("id"), x.Index.ToString());
                var xAttr = new XAttribute(XName.Get("x"), x.X.ToString());
                var yAttr = new XAttribute(XName.Get("y"), x.Y.ToString());
                verticesXmlElements.Add(new XElement(XName.Get("vertex"), indexAttr, xAttr, yAttr));
            });

            graph.Edges.ForEach(x =>
            {
                var sourceIndexAttr = new XAttribute(XName.Get("source"), x.Source.Index);
                var targetIndexAttr = new XAttribute(XName.Get("target"), x.Target.Index);
                var IsDirectedAttr = new XAttribute(XName.Get("directed"), x.IsDirected);
                edgesXmlElements.Add(new XElement(XName.Get("edge"), sourceIndexAttr, targetIndexAttr, IsDirectedAttr));
            });

            var rootElement = new XElement(XName.Get("graph"));
            rootElement.Add(verticesXmlElements.ToArray());
            rootElement.Add(edgesXmlElements.ToArray());
            rootElement.Save(stream);
        }

        private static void PrintMatrix(TextWriter stream, int[,] matrix, int rowsCount, int columnsCount)
        {
            for (int i = 0; i < rowsCount; i++)
            {
                var line = new List<string>();
                for (int j = 0; j < columnsCount; j++)
                {
                    line.Add(matrix[i, j].ToString());
                }
                stream.WriteLine(String.Join(" ", line));
            }
        }
    }
}
