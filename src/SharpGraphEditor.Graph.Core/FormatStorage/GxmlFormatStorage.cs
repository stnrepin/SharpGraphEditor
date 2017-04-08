using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Exceptions;
using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class GxmlFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader stream, IGraph graph)
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
                    var indexAttr = el.Attribute(XName.Get("id"));
                    var xAttr = el.Attribute(XName.Get("x"));
                    var yAttr = el.Attribute(XName.Get("y"));
                    var nameAttr = el.Attribute(XName.Get("name"));
                    var titleAttr = el.Attribute(XName.Get("title"));
                    var colorAttr = el.Attribute(XName.Get("color"));

                    var isSuccessForIndex = Int32.TryParse(indexAttr?.Value, out int index);
                    var isSuccessForX = Double.TryParse(xAttr?.Value, out double x);
                    var isSuccessForY = Double.TryParse(yAttr?.Value, out double y);
                    var isSuccessForColor = Enum.TryParse(colorAttr?.Value, out VertexColor color);

                    var name = nameAttr?.Value;
                    var title = titleAttr?.Value;

                    if (colorAttr == null)
                    {
                        color = VertexColor.White;
                        isSuccessForColor = true;
                    }

                    if (!isSuccessForIndex || !isSuccessForX || !isSuccessForY || !isSuccessForColor)
                    {
                        throw new InputFileFormatException("one or more vertices in GXML file is damaged");
                    }

                    name = String.IsNullOrEmpty(name) ? index.ToString() : name;
                    title = titleAttr == null ? name : title;
                    graph.AddVertex(x, y, index, name, title, color);
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
                var isDirectedAttr = ed.Attribute(XName.Get("directed")) ?? ed.Attribute(XName.Get("IsDirected"));

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

        public override void Save(TextWriter stream, IGraph graph)
        {
            var verticesXmlElements = new List<XElement>();
            var edgesXmlElements = new List<XElement>();

            graph.Vertices.ForEach(vertex =>
            {
                var indexAttr = new XAttribute(XName.Get("id"), vertex.Index.ToString());
                var xAttr = new XAttribute(XName.Get("x"), vertex.X.ToString());
                var yAttr = new XAttribute(XName.Get("y"), vertex.Y.ToString());
                var nameAttr = new XAttribute(XName.Get("name"), vertex.Name);
                var titleAttr = new XAttribute(XName.Get("title"), vertex.Title);
                var colorAttr = new XAttribute(XName.Get("color"), vertex.Color.ToString());

                verticesXmlElements.Add(new XElement(XName.Get("vertex"), indexAttr, xAttr, yAttr, nameAttr, titleAttr, colorAttr));
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
    }
}
