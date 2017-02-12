using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using SharpGraphEditor.Graph.Core;

namespace SharpGraphEditor.Graph.Core
{
    public static class GraphWriter
    {
        public static void ToGxml(string path, IGraph doc)
        {
            var verticesXmlElements = new List<XElement>();
            var edgesXmlElements = new List<XElement>();

            doc.Vertices.ToList().ForEach(x =>
            {
                var indexAttr = new XAttribute(XName.Get("id"), x.Index.ToString());
                var xAttr = new XAttribute(XName.Get("x"), x.X.ToString());
                var yAttr = new XAttribute(XName.Get("y"), x.Y.ToString());
                verticesXmlElements.Add(new XElement(XName.Get("vertex"), indexAttr, xAttr, yAttr));
            });

            doc.Edges.ToList().ForEach(x =>
            {
                var sourceIndexAttr = new XAttribute(XName.Get("source"), x.Source.Index);
                var targetIndexAttr = new XAttribute(XName.Get("target"), x.Target.Index);
                edgesXmlElements.Add(new XElement(XName.Get("edge"), sourceIndexAttr, targetIndexAttr));
            });

            var rootElement = new XElement(XName.Get("graph"));
            rootElement.Add(verticesXmlElements.ToArray());
            rootElement.Add(edgesXmlElements.ToArray());
            rootElement.Save(path);
        }
    }
}
