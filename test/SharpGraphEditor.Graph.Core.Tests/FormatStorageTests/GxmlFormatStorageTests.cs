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
  <edge source=""2"" target=""3""directed=""true"" />
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

            graphMock.Verify(g => g.AddVertex(1, 11, 1, "1", "1"), Times.Once);
            graphMock.Verify(g => g.AddVertex(2, 22, 2, "vertex2", "v2"), Times.Once);
            graphMock.Verify(g => g.AddVertex(3, 33, 3, "3", "v3", VertexColor.Red), Times.Once);
            graphMock.Verify(g => g.AddVertex(4, 44, 4, "4", "4", VertexColor.Green), Times.Once);

            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v1 => v1.Index == 1), It.Is<IVertex>(v2 => v2.Index == 2),  false, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v2 => v2.Index == 2), It.Is<IVertex>(v3 => v3.Index == 3), true, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v3 => v3.Index == 3), It.Is<IVertex>(v4 => v4.Index == 4), false, true), Times.Once);
            graphMock.Verify(g => g.AddEdge(It.Is<IVertex>(v4 => v4.Index == 4), It.Is<IVertex>(v1 => v1.Index == 1), true, true), Times.Once);
        }
    }
}
