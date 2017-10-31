using System;
using System.Collections.Generic;
using System.Globalization;
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
        public double MonitorDpi { get; } = 96;

        public override void Open(TextReader reader, IGraph graph)
        {
            base.Open(reader, graph);

            double graphHeight = 0.0;

            int verticesCount = 0;
            foreach (var line in ReadAllLines(reader))
            {
                var parts = SplitByWhitespacesWithQuotes(line);
                var statementType = parts[0];
                parts = parts.Skip(1).ToArray();
                if (statementType == "graph")
                {
                    graphHeight = ParseStringTo<double>(parts[2]) * MonitorDpi;
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

                    graph.AddVertex(x * MonitorDpi, (graphHeight - y * MonitorDpi), ++verticesCount, name, label);
                }
                else if (statementType == "edge")
                {
                    if (parts.Length < 7)
                    {
                        throw new InputFileFormatException("Properties count of \"edge\" must be more then 7");
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
            var peWriter = new PlainExtWriter(writer);

            var graphHeight = graph.Vertices.Max(x => x.Y) / MonitorDpi;
            var graphWidth = graph.Vertices.Max(x => x.X) / MonitorDpi;
            peWriter.WriteGraph(1.0, graphWidth, graphHeight);
            foreach (var vertex in graph.Vertices)
            {
                peWriter.WriteNode(vertex.Name, vertex.X / MonitorDpi, (vertex.Y + graphHeight) / MonitorDpi, 0, 0, "\"" + vertex.Title + "\"", "default", "default", vertex.Color.ToString().ToLower(), "white");
            }

            foreach (var edge in graph.Edges)
            {
                var points = new List<Tuple<double, double>>(Enumerable.Empty<Tuple<double, double>>());
                peWriter.WriteEdge(edge.Source.Name, edge.Target.Name, points, "1", 0, 0, "default", "black");
            }
            peWriter.WriteEnd();
        }

        private static string[] SplitByWhitespacesWithQuotes(string input)
        {
            return System.Text.RegularExpressions.Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<System.Text.RegularExpressions.Match>()
                .Select(m => m.Value).ToArray();
        }
    }

    internal class PlainExtWriter
    {
        private TextWriter _writer;

        public PlainExtWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public void WriteGraph(double scale, double width, double height)
        {
            var scaleStr = scale.ToString(CultureInfo.InvariantCulture);
            var widthStr = width.ToString(CultureInfo.InvariantCulture);
            var heightStr = height.ToString(CultureInfo.InvariantCulture);

            _writer.WriteLine($"graph {scaleStr} {widthStr} {heightStr}");
        }

        public void WriteNode(string name, double x, double y, double width, double height, string label, string style, string shape, string color, string fillcolor)
        {
            var xStr = x.ToString(CultureInfo.InvariantCulture);
            var yStr = y.ToString(CultureInfo.InvariantCulture);
            var widthStr = width.ToString(CultureInfo.InvariantCulture);
            var heightStr = height.ToString(CultureInfo.InvariantCulture);

            _writer.WriteLine($"node {name} {xStr} {yStr} {widthStr} {heightStr} {label} {style} {shape} {color} {fillcolor}");
        }

        public void WriteEdge(string tail, string head, List<Tuple<double, double>> controlPoints, string label, double xl, double yl, string style, string color)
        {
            var nStr = controlPoints.Count;
            var controlPointsStr = String.Join(" ", controlPoints.Select(x => $"{x.Item1} {x.Item2}"));
            var xlStr = xl.ToString(CultureInfo.InvariantCulture);
            var ylStr = yl.ToString(CultureInfo.InvariantCulture);

            _writer.WriteLine($"edge {tail} {head} {nStr} {controlPointsStr} {label} {xlStr} {ylStr} {style} {color}");
        }

        public void WriteEnd()
        {
            _writer.WriteLine("stop");
        }
    }
}
