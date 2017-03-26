using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core;

namespace SharpGraphEditor.Models
{
    public class GraphRepository
    {
        public string SourceFile { get; private set; }
        public GraphSourceType SourceType { get; private set; }

        public GraphRepository()
        {
            SourceFile = "";
            SourceType = GraphSourceType.None;
        }

        public void LoadFromFile(IGraph graph, string path, GraphSourceType sourceType)
        {
            switch (sourceType)
            {
                case GraphSourceType.None:
                    throw new ArgumentException("Type of source file can't be None");
                case GraphSourceType.Gxml:
                    GraphReader.FromGxml(path, graph);
                    break;
                case GraphSourceType.AdjList:
                    GraphReader.FromAdjList(path, graph);
                    break;
                case GraphSourceType.AdjMatrix:
                    GraphReader.FromAdjMatrix(path, graph);
                    break;
                case GraphSourceType.EdgesList:
                    GraphReader.FromEdgesList(path, graph);
                    break;
                case GraphSourceType.IncidenceMatrix:
                    GraphReader.FromIncidenceMatrix(path, graph);
                    break;
                case GraphSourceType.GraphVizPlainTextExt:
                    GraphReader.FromGraphVizPlainTextExt(path, graph);
                    break;
                default:
                    throw new NotSupportedException($"{sourceType.ToString()} not support");
            }

            SourceFile = path;
            SourceType = sourceType;
        }

        public void LoadFromText(IGraph graph, string text, GraphSourceType sourceType)
        {
            using (var stringReader = new StringReader(text))
            {
                switch (sourceType)
                {
                    case GraphSourceType.None:
                        throw new ArgumentException("Type of source file can't be None");
                    case GraphSourceType.Gxml:
                        GraphReader.FromGxml(stringReader, graph);
                        break;
                    case GraphSourceType.AdjList:
                        GraphReader.FromAdjList(stringReader, graph);
                        break;
                    case GraphSourceType.AdjMatrix:
                        GraphReader.FromAdjMatrix(stringReader, graph);
                        break;
                    case GraphSourceType.EdgesList:
                        GraphReader.FromEdgesList(stringReader, graph);
                        break;
                    case GraphSourceType.IncidenceMatrix:
                        GraphReader.FromIncidenceMatrix(stringReader, graph);
                        break;
                    case GraphSourceType.GraphVizPlainTextExt:
                        GraphReader.FromGraphVizPlainTextExt(stringReader, graph);
                        break;
                    default:
                        throw new NotSupportedException($"{sourceType.ToString()} not support");
                }
            }
        }

        public void SaveToFile(IGraph graph, string path, GraphSourceType sourceType)
        {
            switch (sourceType)
            {
                case GraphSourceType.None:
                    throw new ArgumentException("Type of source file can't be none");
                case GraphSourceType.Gxml:
                    GraphWriter.ToGxml(path, graph);
                    break;
                case GraphSourceType.AdjList:
                    GraphWriter.ToAdjList(path, graph);
                    break;
                case GraphSourceType.AdjMatrix:
                    GraphWriter.ToAdjMatrix(path, graph);
                    break;
                case GraphSourceType.EdgesList:
                    GraphWriter.ToEdgesList(path, graph);
                    break;
                case GraphSourceType.IncidenceMatrix:
                    GraphWriter.ToIncidenceMatrix(path, graph);
                    break;
                default:
                    throw new NotSupportedException($"{sourceType.ToString()} not support");
            }
            
            SourceFile = path;
            SourceType = sourceType;
        }

        public string PresentAsText(IGraph graph, GraphSourceType sourceType)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                switch (sourceType)
                {
                    case GraphSourceType.None:
                        throw new ArgumentException("Type of source file can't be none");
                    case GraphSourceType.Gxml:
                        GraphWriter.ToGxml(stringWriter, graph);
                        break;
                    case GraphSourceType.AdjList:
                        GraphWriter.ToAdjList(stringWriter, graph);
                        break;
                    case GraphSourceType.AdjMatrix:
                        GraphWriter.ToAdjMatrix(stringWriter, graph);
                        break;
                    case GraphSourceType.EdgesList:
                        GraphWriter.ToEdgesList(stringWriter, graph);
                        break;
                    case GraphSourceType.IncidenceMatrix:
                        GraphWriter.ToIncidenceMatrix(stringWriter, graph);
                        break;
                    default:
                        throw new NotSupportedException($"{sourceType.ToString()} not support");
                }
            }
            return sb.ToString();
        }
    }
}
