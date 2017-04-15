using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Exceptions;
using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public class HierarchicalRtfFormatStorage : BaseFormatStorage
    {
        public override void Open(TextReader reader, IGraph graph)
        {
            throw new NotImplementedException("this format supports only saving");
        }

        public override void Save(TextWriter writer, IGraph graph)
        {
            if (!graph.IsDirected)
            {
                throw new InvalidGraphFormatException("graph must be directed");
            }

            var rootVertices = graph.Vertices.ToArray();

            foreach (var column in graph.ToAdjList())
            {
                var parent = column.Key;
                foreach (var child in column.Value)
                {
                    rootVertices[child.Index - 1] = null;
                }
            }

            rootVertices = rootVertices.Where(x => x != null).ToArray();

            if (rootVertices.Length == 0)
            {
                throw new InvalidGraphFormatException("graph must have some vertices without parent");
            }

            var rtfWriter = new RtfWriter(writer);
            rtfWriter.WriteDocumentStart();

            var stack = new Stack<Tuple<IVertex, int>>();
            int indent = -1;
            var dfs = new Algorithms.Helpers.DepthFirstSearch(graph)
            {
                ProcessVertexEarly = (_) =>
                {
                    indent++;
                },
                ProcessVertexLate = (v) =>
                {
                    stack.Push(new Tuple<IVertex, int>(v, indent));
                    indent--;
                }
            };
            foreach (var root in rootVertices)
            {
                dfs.Run(root);
                while (stack.Count > 0)
                {
                    var col = stack.Pop();
                    var vertex = col.Item1;
                    var indentLevel = col.Item2;

                    rtfWriter.WriteLine(vertex.Name, indentLevel, vertex.Color.ToString());
                }
            }
            rtfWriter.WriteDocumentEnd();
        }

        private class RtfWriter
        {
            private readonly string ColorTable = "{\\colortbl ;" + Environment.NewLine +
                                            "\\red0\\green0\\blue0;" + Environment.NewLine +
                                            "\\red0\\green0\\blue255;" + Environment.NewLine +
                                            "\\red0\\green255\\blue0;" + Environment.NewLine +
                                            "\\red255\\green0\\blue0;" + Environment.NewLine +
                                            "\\red128\\green128\\blue128;}";
            private readonly Dictionary<string, string> ColorMap = new Dictionary<string, string>()
            {
                ["Black"] = "0",
                ["White"] = "1",
                ["Blue"] = "2",
                ["Green"] = "3",
                ["Red"] = "4",
                ["Gray"] = "5"
            };

            private TextWriter _textWriter;

            public RtfWriter(TextWriter textWriter)
            {
                _textWriter = textWriter;
            }

            public void WriteDocumentStart()
            {
                _textWriter.WriteLine("{\\rtf1 " + Environment.NewLine + ColorTable + Environment.NewLine);
            }

            public void WriteDocumentEnd()
            {
                _textWriter.WriteLine(Environment.NewLine + "}");
            }

            public void WriteLine(string text, int indentLevel, string colorName)
            {
                var indent = String.Empty;
                for (int i = 0; i < indentLevel; i++)
                {
                    indent += "\\tab";
                }

                var rootStr = String.Join("", text.Select(x => "\\u" + ((int)x).ToString() + "?"));
                _textWriter.Write("\\line " + indent + " \\bullet  ");
                _textWriter.WriteLine("{\\cf" + ColorMap[colorName] + " " + rootStr + "\\cf0" + "}");
            }
        }
    }
}
