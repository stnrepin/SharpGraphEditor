using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Exceptions;

namespace SharpGraphEditor.Graph.Core
{
    public static class GraphReader
    {
        // Rewrite for format https://ru.wikipedia.org/wiki/GraphML
        // Serialization
        public static void FromGxml(string path, IGraph graph)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("GXML file not found");

            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }

            XDocument doc;
            try
            {
                doc = XDocument.Load(path);
            }
            catch (Exception ex)
            {
                throw new GxmlFileFormatException("Error during parsing of Gxml file", ex);
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
                        throw new GxmlFileFormatException("one or more vertices in GXML file is damaged");
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
                    throw new GxmlFileFormatException("one or more edges in GXML file is damaged");
                }

                if (sourceIndex != targetIndex)
                {
                    var sourceVertex = graph.FindVertexByIndex(sourceIndex);
                    var targetVertex = graph.FindVertexByIndex(targetIndex);

                    if (sourceVertex == null || targetVertex == null)
                    {
                        throw new GxmlFileFormatException("one or more edges in GXML file is damaged");
                    }

                    graph.AddEdge(sourceVertex, targetVertex, isDirected);
                }
            }
        }
    }
}
