using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Exceptions;
using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Graph.Core
{
    public static class GraphReader
    {
        public static void FromIncidenceMatrix(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        FromIncidenceMatrix(stream, graph);
                    }
                }
            }
            catch (InputFileFormatException)
            {
                graph.Clear();
                throw;
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of file with incidence matrix an error occured", ex);
            }
        }

        public static void FromIncidenceMatrix(StreamReader stream, IGraph graph)
        {
            var incMatrix = ReadAllLines(stream).Select(x => x.Split(' ')
                                                              .ToList())
                                                .ToList();
            var verticesCount = incMatrix.Count;
            if (verticesCount == 0)
                return;

            var edgesCount = incMatrix.First().Count;
            for (int edgeIndex = 0; edgeIndex < edgesCount; edgeIndex++)
            {
                IVertex source = null;
                IVertex target = null;
                for (int vertexIndex = 0; vertexIndex < verticesCount; vertexIndex++)
                {
                    if (incMatrix[vertexIndex][edgeIndex] == "1")
                    {
                        if (target != null)
                        {
                            source = graph.AddVertex(vertexIndex + 1);
                        }
                        else
                        {
                            target = graph.AddVertex(vertexIndex + 1);
                        }
                    }
                    else if (incMatrix[vertexIndex][edgeIndex] == "-1")
                    {
                        source = graph.AddVertex(vertexIndex + 1);
                    }
                }
                System.Diagnostics.Debug.WriteLine($"{source} -> {target}");
                graph.AddEdge(source, target, directedIfReversedExisting: true);
            }

        }

        public static void FromEdgesList(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        FromEdgesList(stream, graph);
                    }
                }
            }
            catch (InputFileFormatException)
            {
                graph.Clear();
                throw;
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of file with edges list an error occured", ex);
            }
        }

        public static void FromEdgesList(StreamReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            var lines = ReadAllLines(stream).ToArray();
            foreach (var line in lines)
            {
                var edges = line.Split(' ').Select(x => StringToInt(x)).ToArray();
                if (edges.Length != 2)
                {
                    throw new ArgumentOutOfRangeException("Bad format of edges list file");
                }

                var v1 = graph.AddVertex(edges[0]);
                var v2 = graph.AddVertex(edges[1]);
                graph.AddEdge(v1, v2, directedIfReversedExisting: true);
            }
        }

        public static void FromAdjMatrix(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        FromAdjMatrix(stream, graph);
                    }
                }
            }
            catch (InputFileFormatException)
            {
                graph.Clear();
                throw;
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of file with adj matrix an error occured", ex);
            }
        }

        public static void FromAdjMatrix(StreamReader stream, IGraph graph)
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
                        graph.AddEdge(v1, v2, directedIfReversedExisting: true);
                    }
                }
            }
        }

        public static void FromAdjList(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        FromAdjList(stream, graph);
                    }
                }
            }
            catch (InputFileFormatException)
            {
                graph.Clear();
                throw;
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of file with adj list an error occured", ex);
            }
        }

        public static void FromAdjList(StreamReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            var lines = ReadAllLines(stream);

            var list = new List<IEnumerable<int>>();
            foreach (var line in lines)
            {
                var verticesIndexes = line.Split(' ').Select(x => StringToInt(x));
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
                    graph.AddEdge(parentVertex, targetVertex, directedIfReversedExisting: true);
                }
            }
        }

        public static void FromGxml(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        FromGxml(stream, graph);
                    }
                }
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of gxml file an error occured", ex);
            }
        }

        // Rewrite for format https://ru.wikipedia.org/wiki/GraphML
        // Serialization
        public static void FromGxml(StreamReader stream, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(stream);
            }
            catch (Exception ex)
            {
                throw new InputFileFormatException("Error during parsing of Gxml file", ex);
            }

            var edgeXmlElements = new List<XElement>();
            foreach (var el in doc.Root.Elements())
            {
                if (el.Name == "vertex" || el.Name == "Vertex")
                {
                    var indexAttr = el.Attribute(XName.Get("id")) ?? el.Attribute(XName.Get("Id"));
                    var xAttr = el.Attribute(XName.Get("x")) ?? el.Attribute(XName.Get("X"));
                    var yAttr = el.Attribute(XName.Get("y")) ?? el.Attribute(XName.Get("Y"));

                    var isSuccessForIndex = Int32.TryParse(indexAttr?.Value, out int index);
                    var isSuccessForX = Double.TryParse(xAttr?.Value, out double x);
                    var isSuccessForY = Double.TryParse(yAttr?.Value, out double y);

                    if (!isSuccessForIndex || !isSuccessForX || !isSuccessForY)
                    {
                        throw new InputFileFormatException("one or more vertices in GXML file is damaged");
                    }

                    graph.AddVertex(x, y, index);
                }
                else if (el.Name == "edge" || el.Name == "Edge")
                {
                    edgeXmlElements.Add(el);
                }
            }

            foreach (var ed in edgeXmlElements)
            {
                var sourceIndexAttr = ed.Attribute(XName.Get("source")) ?? ed.Attribute(XName.Get("Source"));
                var targetIndexAttr = ed.Attribute(XName.Get("target")) ?? ed.Attribute(XName.Get("Target"));
                var isDirectedAttr = ed.Attribute(XName.Get("isDirected")) ?? ed.Attribute(XName.Get("IsDirected"));

                var isSuccessForIsDirected = Boolean.TryParse(isDirectedAttr?.Value, out bool isDirected);
                var isSuccessForSourceIndex = Int32.TryParse(sourceIndexAttr?.Value, out int sourceIndex);
                var isSuccessForTargetIndex = Int32.TryParse(targetIndexAttr?.Value, out int targetIndex);

                if (isDirectedAttr == null)
                {
                    isDirected = false;
                    isSuccessForIsDirected = true;
                }

                if (!isSuccessForSourceIndex || !isSuccessForTargetIndex || !isSuccessForIsDirected)
                {
                    throw new InputFileFormatException("one or more edges in GXML file is damaged");
                }

                if (sourceIndex != targetIndex)
                {
                    var sourceVertex = graph.FindVertexByIndex(sourceIndex);
                    var targetVertex = graph.FindVertexByIndex(targetIndex);

                    if (sourceVertex == null || targetVertex == null)
                    {
                        throw new InputFileFormatException("one or more edges in GXML file is damaged");
                    }

                    graph.AddEdge(sourceVertex, targetVertex, isDirected);
                }
            }
        }

        private static int StringToInt(string str)
        {
            if (!Int32.TryParse(str, out int i))
            {
                throw new InputFileFormatException("Index of vertex must be integer");
            }
            return i;
        }

        private static IEnumerable<string> ReadAllLines(StreamReader stream)
        {
            while (!stream.EndOfStream)
            {
                yield return stream.ReadLine();
            }
        }
    }
}
