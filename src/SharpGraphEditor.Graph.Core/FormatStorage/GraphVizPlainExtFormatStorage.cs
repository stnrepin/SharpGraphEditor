using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Exceptions;
using static SharpGraphEditor.Graph.Core.Utils.FormatStorageUtils;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    // http://www.graphviz.org/doc/info/output.html#d:plain-ext
    public class GraphVizPlainExtFormatStorage : BaseFormatStorage
    {
        public double MonitorDpi { get; set; } = 96;

        public override void Open(TextReader reader, IGraph graph)
        {
            base.Open(reader, graph);

            int verticesCount = 0;
            foreach (var line in ReadAllLines(reader))
            {
                var parts = SplitByWhitespacesWithQuotes(line);
                var statementType = parts[0];
                parts = parts.Skip(1).ToArray();
                if (statementType == "graph")
                {
                    if (parts.Length != 3)
                    {
                        throw new InputFileFormatException("Properties count of \"graph\" must be 3");
                    }
                }
                else if (statementType == "node")
                {
                    if (parts.Length != 10)
                    {
                        throw new InputFileFormatException("Properties count of \"node\" must be 10");
                    }

                    var name = parts[0];
                    var x = ParseStringTo<double>(parts[1]);
                    var y = ParseStringTo<double>(parts[2]);
                    var label = parts[5];
                    if (label.StartsWith("\"") && label.EndsWith("\""))
                    {
                        label = label.Substring(1, label.Length - 2);
                    }

                    graph.AddVertex(x * MonitorDpi, y * MonitorDpi, ++verticesCount, name, label);
                }
                else if (statementType == "edge")
                {
                    if (parts.Length < 7)
                    {
                        throw new InputFileFormatException("Properties count of \"node\" must be more then 7");
                    }
                    var tailName = parts[0];
                    var headName = parts[1];

                    var e = graph.AddEdge(graph.FindVertexByName(tailName), graph.FindVertexByName(headName));
                }
                else if (statementType == "stop")
                {
                    break;
                }
            }
        }

        public override void Save(TextWriter writer, IGraph graph)
        {
            throw new NotImplementedException("this format supports only opening");
        }

        private static string[] SplitByWhitespacesWithQuotes(string input)
        {
            return System.Text.RegularExpressions.Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Value).ToArray();
        }
    }
}
