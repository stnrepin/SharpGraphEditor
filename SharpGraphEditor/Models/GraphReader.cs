using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Models.Exceptions;

namespace SharpGraphEditor.Models
{
    public static class GraphReader
    {
        // Переписать под формат https://ru.wikipedia.org/wiki/GraphML
        // Serialization
        public static List<IGraphElement> FromGxml(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException("GXML file not found");

            var vertices = new List<Vertex>();
            var edges = new List<Edge>();

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

                    vertices.Add(new Vertex(x, y, index));
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

                var isSuccessForSourceIndex = Int32.TryParse(sourceIndexAttr?.Value, out int sourceIndex);
                var isSuccessForTargetIndex = Int32.TryParse(targetIndexAttr?.Value, out int targetIndex);

                if (!isSuccessForSourceIndex || !isSuccessForTargetIndex)
                {
                    throw new GxmlFileFormatException("one or more edges in GXML file is damaged");
                }

                if (sourceIndex != targetIndex)
                {
                    var sourceVertex = FindVertexByIndex(vertices, sourceIndex);
                    var targetVertex = FindVertexByIndex(vertices, targetIndex);

                    if (sourceVertex == null || targetVertex == null)
                    {
                        throw new GxmlFileFormatException("one or more edges in GXML file is damaged");
                    }

                    edges.Add(new Edge(sourceVertex, targetVertex, false)); // TODO: direction
                }
            }

            var res = new List<IGraphElement>();
            res.AddRange(vertices.OrderBy(x => x.Index));
            res.AddRange(edges);
            return res;
        }

        private static Vertex FindVertexByIndex(List<Vertex> vertices, int index)
        {
            foreach (var v in vertices)
            {
                if (v.Index == index) return v;
            }
            return null;
        }
    }
}
