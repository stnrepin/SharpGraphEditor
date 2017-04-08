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
    public class GxmlFormatStorageTests
    {
        [TestMethod]
        public void Open_ValidText_Graph()
        {
            var text = @"<?xml version=""1.0"" encoding=""utf-8""?>
<graph>
  <vertex id=""1"" x=""1"" y=""11"" />
  <vertex id=""2"" x=""2"" y=""22"" title=""v2"" name=""vertex2""/>
  <vertex id=""3"" x=""3"" y=""33"" title=""v3"" color=""Red""/>
  <vertex id=""4"" x=""4"" y=""44"" color=""Green""/>
  <edge source=""1"" target=""2"" />
  <edge source=""2"" target=""3"" directed=""true"" />
  <edge source=""3"" target=""4"" directed=""false"" />
  <edge source=""4"" target=""1"" directed=""true"" />
</graph>";

            var storage = new GxmlFormatStorage();
            var graphMock = new Mock<IGraph>();

            graphMock.Setup(g => g.FindVertexByIndex(It.IsAny<int>()))
                   .Returns<int>(index => Mock.Of<IVertex>(v => v.Index == index));

            using (var sr = new System.IO.StringReader(text))
            {
                storage.Open(sr, graphMock.Object);
            }

            graphMock.Verify(g => g.AddVertex(1, 11, 1, "1", "1", VertexColor.White), Times.Once);
            graphMock.Verify(g => g.AddVertex(2, 22, 2, "vertex2", "v2", VertexColor.White), Times.Once);
            graphMock.Verify(g => g.AddVertex(3, 33, 3, "3", "v3", VertexColor.Red), Times.Once);
            graphMock.Verify(g => g.AddVertex(4, 44, 4, "4", "4", VertexColor.Green), Times.Once);

            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v1 => v1.Index == 1), It.Is<IVertex>(v2 => v2.Index == 2),  false, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v2 => v2.Index == 2), It.Is<IVertex>(v3 => v3.Index == 3), true, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v3 => v3.Index == 3), It.Is<IVertex>(v4 => v4.Index == 4), false, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v4 => v4.Index == 4), It.Is<IVertex>(v1 => v1.Index == 1), true, true), Times.Once);
        }

        [TestMethod]
        public void Save_ValidGraph_Text()
        {
            var expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<graph>
  <vertex id=""1"" x=""1"" y=""11"" name=""1"" title=""1"" color=""White"" />
  <vertex id=""2"" x=""2"" y=""22"" name=""vertex2"" title=""v2"" color=""White"" />
  <vertex id=""3"" x=""3"" y=""33"" name=""3"" title=""v3"" color=""Red"" />
  <vertex id=""4"" x=""4"" y=""44"" name=""4"" title=""4"" color=""Green"" />
  <edge source=""1"" target=""2"" directed=""false"" />
  <edge source=""2"" target=""3"" directed=""true"" />
  <edge source=""3"" target=""4"" directed=""false"" />
  <edge source=""4"" target=""1"" directed=""true"" />
</graph>";

            var storage = new GxmlFormatStorage();

            var v1 = Mock.Of<IVertex>(v => v.Index == 1 && v.Name == "1" && v.Title == "1" && v.X == 1 && v.Y == 11 && v.Color == VertexColor.White);
            var v2 = Mock.Of<IVertex>(v => v.Index == 2 && v.Name == "vertex2" && v.Title == "v2" && v.X == 2 && v.Y == 22 && v.Color == VertexColor.White);
            var v3 = Mock.Of<IVertex>(v => v.Index == 3 && v.Name == "3" && v.Title == "v3" && v.X == 3 && v.Y == 33 && v.Color == VertexColor.Red);
            var v4 = Mock.Of<IVertex>(v => v.Index == 4 && v.Name == "4" && v.Title == "4" && v.X == 4 && v.Y == 44 && v.Color == VertexColor.Green);

            var e12 = Mock.Of<IEdge>(e => e.Source == v1 && e.Target == v2 && e.IsDirected == false);
            var e23 = Mock.Of<IEdge>(e => e.Source == v2 && e.Target == v3 && e.IsDirected == true);
            var e34 = Mock.Of<IEdge>(e => e.Source == v3 && e.Target == v4 && e.IsDirected == false);
            var e41 = Mock.Of<IEdge>(e => e.Source == v4 && e.Target == v1 && e.IsDirected == true);

            var graphMock = new Mock<IGraph>();
            graphMock.Setup(g => g.Vertices).Returns(new IVertex[] { v1, v2, v3, v4 });
            graphMock.Setup(g => g.Edges).Returns(new IEdge[] { e12, e23, e34, e41 });

            var sb = new StringBuilder();
            using (var sw = new System.IO.StringWriter(sb))
            {
                storage.Save(sw, graphMock.Object);
            }

            Assert.AreEqual(expectedText, sb.ToString());
        }
    }
}
