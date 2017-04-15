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
    public class HierarchicalRtfFormatStorageTests
    {
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void Open_ValidText_RaiseException()
        {
            var storage = new HierarchicalRtfFormatStorage();

            var text = "{\\rtf}";
            var graphMock = new Mock<IGraph>();

            using (var sr = new System.IO.StringReader(text))
            {
                storage.Open(sr, graphMock.Object);
            }
        }

        [TestMethod]
        public void Save_ValidGraph_Text()
        {
            var expectedText = @"{\rtf1 
{\colortbl ;
\red0\green0\blue0;
\red0\green0\blue255;
\red0\green255\blue0;
\red255\green0\blue0;
\red128\green128\blue128;}

\line  \bullet  {\cf1 \u97?\cf0}
\line \tab \bullet  {\cf2 \u98?\cf0}
\line \tab\tab \bullet  {\cf3 \u100?\cf0}
\line \tab\tab \bullet  {\cf1 \u99?\cf0}

}
";
            var storage = new HierarchicalRtfFormatStorage();

            var v1 = Mock.Of<IVertex>(v => v.Index == 1 && v.Name == "a" && v.Title == "a" && v.X == 200 && v.Y == 50 && v.Color == VertexColor.White);
            var v2 = Mock.Of<IVertex>(v => v.Index == 2 && v.Name == "b" && v.Title == "b" && v.X == 200 && v.Y == 100 && v.Color == VertexColor.Blue);
            var v3 = Mock.Of<IVertex>(v => v.Index == 3 && v.Name == "c" && v.Title == "c" && v.X == 250 && v.Y == 125 && v.Color == VertexColor.White);
            var v4 = Mock.Of<IVertex>(v => v.Index == 4 && v.Name == "d" && v.Title == "d" && v.X == 200 && v.Y == 150 && v.Color == VertexColor.Green);

            var adjList = new Dictionary<IVertex, IEnumerable<IVertex>>()
            {
                [v1] = new IVertex[] { v2 },
                [v2] = new IVertex[] { v3, v4},
                [v3] = new IVertex[0],
                [v4] = new IVertex[] { v3 }
            };

            var graphMock = new Mock<IGraph>();
            graphMock.Setup(g => g.IsDirected).Returns(true);
            graphMock.Setup(g => g.Vertices).Returns(new IVertex[] { v1, v2, v3, v4 });
            graphMock.Setup(g => g.ToAdjList()).Returns(adjList);

            var graph = graphMock.Object;

            var sb = new StringBuilder();
            using (var sw = new System.IO.StringWriter(sb))
            {
                storage.Save(sw, graph);
            }

            Assert.AreEqual(expectedText, sb.ToString());
        }
    }
}
