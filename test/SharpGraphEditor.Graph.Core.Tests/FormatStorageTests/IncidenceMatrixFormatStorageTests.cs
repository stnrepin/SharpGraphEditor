using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpGraphEditor.Graph.Core.FormatStorage;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Tests.FormatStorageTests
{
    [TestClass]
    public class IncidenceMatrixFormatStorageTests
    {
        [TestMethod]
        public void Open_ValidText_Graph()
        {
            var text = @"1 1 0 0
-1 0 1 1
0 1 0 -1
0 0 -1 0";

            var storage = new IncidenceMatrixFormatStorage();
            var graphMock = new Mock<IGraph>();

            graphMock.Setup(g => g.AddVertex(It.IsAny<int>())).Returns<int>(index => Mock.Of<IVertex>(v => v.Index == index));

            using (var sr = new System.IO.StringReader(text))
            {
                storage.Open(sr, graphMock.Object);
            }

            graphMock.Verify(g => g.AddVertex(1), Times.Exactly(2));
            graphMock.Verify(g => g.AddVertex(2), Times.Exactly(3));
            graphMock.Verify(g => g.AddVertex(3), Times.Exactly(2));
            graphMock.Verify(g => g.AddVertex(4), Times.Exactly(1));

            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v => v.Index == 1), It.Is<IVertex>(v => v.Index == 2), true, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v => v.Index == 1), It.Is<IVertex>(v => v.Index == 3), false, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v => v.Index == 2), It.Is<IVertex>(v => v.Index == 4), true, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v => v.Index == 2), It.Is<IVertex>(v => v.Index == 3), true, true), Times.Once);
        }

        [TestMethod]
        public void Save_ValidGraph_Text()
        {
            var expectedText = @"1 1 0 0
-1 0 1 1
0 1 0 -1
0 0 -1 0" + Environment.NewLine;

            var storage = new IncidenceMatrixFormatStorage();
            var graphMock = new Mock<IGraph>();

            var v1 = Mock.Of<IVertex>(v => v.Index == 1 && v.Name == "1" && v.Title == "1" && v.X == 1 && v.Y == 11 && v.Color == VertexColor.White);
            var v2 = Mock.Of<IVertex>(v => v.Index == 2 && v.Name == "vertex2" && v.Title == "v2" && v.X == 2 && v.Y == 22 && v.Color == VertexColor.White);
            var v3 = Mock.Of<IVertex>(v => v.Index == 3 && v.Name == "3" && v.Title == "v3" && v.X == 3 && v.Y == 33 && v.Color == VertexColor.Red);
            var v4 = Mock.Of<IVertex>(v => v.Index == 4 && v.Name == "4" && v.Title == "4" && v.X == 4 && v.Y == 44 && v.Color == VertexColor.Green);

            var e1 = Mock.Of<IEdge>(e => e.Source == v1 && e.Target == v2 && e.IsDirected == true);
            var e2 = Mock.Of<IEdge>(e => e.Source == v1 && e.Target == v3);
            var e3 = Mock.Of<IEdge>(e => e.Source == v2 && e.Target == v4 && e.IsDirected == true);
            var e4 = Mock.Of<IEdge>(e => e.Source == v2 && e.Target == v3 && e.IsDirected == true);

            graphMock.Setup(g => g.Vertices).Returns(new IVertex[] { v1, v2, v3, v4 });
            graphMock.Setup(g => g.Edges).Returns(new IEdge[] { e1, e2, e3, e4 });

            var sb = new StringBuilder();
            using (var sw = new System.IO.StringWriter(sb))
            {
                storage.Save(sw, graphMock.Object);
            }

            Assert.AreEqual(expectedText, sb.ToString());
        }
    }
}
