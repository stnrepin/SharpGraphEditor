using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.FormatStorage;

namespace SharpGraphEditor.Graph.Core.Tests.FormatStorageTests
{
    [TestClass]
    public class GraphVizPlainTextExtFormatStorageTests
    {
        [TestMethod]
        public void Open_ValidText_Graph()
        {
            var text = "graph <scale> <width> <height>\n" +
                       "node vertex1 1 1.5 <width> <height> v1 <style> <shape> <color> <fillcolor>\n" +
                       "node vertex2 2 2.5 <width> <height> v2 <style> <shape> <color> <fillcolor>\n" +
                       "edge vertex2 vertex1 <n> <x1> <y1> <..> <xn> <yn> [<label> <xl> <yl>] <style> <color>\n" +
                       "stop";

            var storage = new GraphVizPlainTextExtFormatStorage();
            var graphMock = new Mock<IGraph>();

            graphMock.Setup(g => g.FindVertexByName(It.IsAny<string>()))
                   .Returns<string>(name => Mock.Of<IVertex>(v => v.Name == name));

            var dpi = storage.MonitorDpi;
            var graph = graphMock.Object;

            using (var sr = new System.IO.StringReader(text))
            {
                storage.Open(sr, graph);
            }

            graphMock.Verify(g => g.AddVertex(dpi * 1, dpi * 1.5, 1, "vertex1", "v1"), Times.Once);
            graphMock.Verify(g => g.AddVertex(dpi * 2, dpi * 2.5, 2, "vertex2", "v2"), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v2 => v2.Name == "vertex2"), It.Is<IVertex>(v1 => v1.Name == "vertex1"), false, true), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Save_ValidGraph_RaiseException()
        {
            var storage = new GraphVizPlainTextExtFormatStorage();
            var graphMock = new Mock<IGraph>();

            var sb = new StringBuilder();
            using (var sw = new System.IO.StringWriter(sb))
            {
                storage.Save(sw, graphMock.Object);
            }
        }
    }
}
