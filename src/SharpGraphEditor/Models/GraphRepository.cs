using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.FormatStorage;

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
                    (new GxmlFormatStorage()).Open(path, graph);
                    break;
                case GraphSourceType.AdjList:
                    (new AdjListFormatStorage()).Open(path, graph);
                    break;
                case GraphSourceType.AdjMatrix:
                    (new AdjMatrixFormatStorage()).Open(path, graph);
                    break;
                case GraphSourceType.EdgesList:
                    (new EdgesListFormatStorage()).Open(path, graph);
                    break;
                case GraphSourceType.IncidenceMatrix:
                    (new IncidenceMatrixFormatStorage()).Open(path, graph);
                    break;
                case GraphSourceType.GraphVizPlainExt:
                    (new GraphVizPlainExtFormatStorage()).Open(path, graph);
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
                        (new GxmlFormatStorage()).Open(stringReader, graph);
                        break;
                    case GraphSourceType.AdjList:
                        (new AdjListFormatStorage()).Open(stringReader, graph);
                        break;
                    case GraphSourceType.AdjMatrix:
                        (new AdjMatrixFormatStorage()).Open(stringReader, graph);
                        break;
                    case GraphSourceType.EdgesList:
                        (new EdgesListFormatStorage()).Open(stringReader, graph);
                        break;
                    case GraphSourceType.IncidenceMatrix:
                        (new IncidenceMatrixFormatStorage()).Open(stringReader, graph);
                        break;
                    case GraphSourceType.GraphVizPlainExt:
                        (new GraphVizPlainExtFormatStorage()).Open(stringReader, graph);
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
                    (new GxmlFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.AdjList:
                    (new AdjListFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.AdjMatrix:
                    (new AdjMatrixFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.EdgesList:
                    (new EdgesListFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.IncidenceMatrix:
                    (new IncidenceMatrixFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.GraphVizPlainExt:
                    (new GraphVizPlainExtFormatStorage()).Save(path, graph);
                    break;
                case GraphSourceType.HierarchicalRtf:
                    (new HierarchicalRtfFormatStorage()).Save(path, graph);
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
                        (new GxmlFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.AdjList:
                        (new AdjListFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.AdjMatrix:
                        (new AdjMatrixFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.EdgesList:
                        (new EdgesListFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.IncidenceMatrix:
                        (new IncidenceMatrixFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.GraphVizPlainExt:
                        (new GraphVizPlainExtFormatStorage()).Save(stringWriter, graph);
                        break;
                    case GraphSourceType.HierarchicalRtf:
                        (new HierarchicalRtfFormatStorage()).Save(stringWriter, graph);
                        break;
                    default:
                        throw new NotSupportedException($"{sourceType.ToString()} not support");
                }
            }
            return sb.ToString();
        }

        public string GetFilterForSourceFileType(GraphSourceType fileType)
        {
            var filter = String.Empty;
            switch (fileType)
            {
                case GraphSourceType.Gxml:
                    filter = "GXML files (*.gxml) | *.gxml";
                    break;
                case GraphSourceType.AdjList:
                case GraphSourceType.AdjMatrix:
                case GraphSourceType.EdgesList:
                case GraphSourceType.IncidenceMatrix:
                case GraphSourceType.GraphVizPlainExt:
                    filter = "TXT files (*.txt) | *.txt";
                    break;
                case GraphSourceType.HierarchicalRtf:
                    filter = "Rtf file (*.rtf) | *.rtf";
                    break;
                default:
                    throw new ArgumentException("Unknown file type");
            }
            return filter;
        }
    }
}
