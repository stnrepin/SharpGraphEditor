using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Models
{
    public enum GraphSourceFileType
    {
        None,
        Gxml,
        AdjList,
        AdjMatrix,
        EdgesList,
        IncidenceMatrix
    }

    public class GraphDocument : PropertyChangedBase, IGraph, ICloneable
    {
        // Fields
        //
        private bool _isModified;

        public string SourceFile { get; private set; }
        public GraphSourceFileType SourceFileType { get; private set; }

        public ObservableCollection<IVertex> ObservableVertices { get; private set; }
        public ObservableCollection<IEdge> ObservableEdges { get; private set; }

        public IEnumerable<IVertex> Vertices => (ObservableVertices);
        public IEnumerable<IEdge> Edges => (ObservableEdges);

        // Constructor
        //
        public GraphDocument()
        {
            ObservableVertices = new ObservableCollection<IVertex>();
            ObservableEdges = new ObservableCollection<IEdge>();

            IsModified = false;
            SourceFile = "";
            SourceFileType = GraphSourceFileType.None;
        }


        // Public properties 
        //
        public bool IsDirected
        {
            get
            {
                if (ObservableEdges.Count == 0)
                {
                    return false;
                }
                return ObservableEdges.All(x => x.IsDirected);
            }
            set { ObservableEdges.ForEach(x => x.IsDirected = value); }
        }

        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                if (_isModified == value) return;
                _isModified = value;
                NotifyOfPropertyChange(() => IsModified);
            }
        }

        // Public methods
        //

        public IVertex AddVertex(int index)
        {
            var v = AddVertex(0, 0, index);
            v.HasPosition = false;
            return v;
        }

        public IVertex AddVertex(double x, double y)
        {
            return AddVertex(x, y, GetNewVertexIndex());
        }

        public IVertex AddVertex(double x, double y, int index)
        {
            var existingVertex = FindVertexByIndex(index);
            if (existingVertex != null)
            {
                return existingVertex;
            }
            var newVertex = new Vertex(x, y, index);
            Execute.OnUIThread(() => ObservableVertices.Add(newVertex));
            IsModified = true;
            return newVertex;
        }

        public IEdge AddEdge(IVertex source, IVertex target)
        {
            return AddEdge(source, target, IsDirected);
        }

        public IEdge AddEdge(IVertex source, IVertex target, bool isDirected, bool directedIfReversedExisting = false)
        {
            if (source == target) return null;

            var isEdgeExist = ObservableEdges.Any(x => x.Source == source && x.Target == target);

            if (!isEdgeExist)
            {
                var reversedEdge = ObservableEdges.FirstOrDefault(x => x.Source == target && x.Target == source);
                if (reversedEdge != null)
                {
                    if (directedIfReversedExisting)
                    {
                        reversedEdge.IsDirected = true;
                        IsModified = true;
                        return reversedEdge;
                    }
                }
                else
                {
                    var newEdge = new Edge(source, target, isDirected);
                    Execute.OnUIThread(() => ObservableEdges.Add(newEdge));
                    IsModified = true;
                    return newEdge;
                }
            }
            return null;
        }

        public void Remove(IGraphElement element)
        {
            if (element is IVertex)
            {
                var vertex = element as IVertex;
                Execute.OnUIThread(() => ObservableVertices.Remove(vertex));
                ObservableEdges.Where(x => x.Source == vertex || x.Target == vertex).ToList()
                    .ForEach(x => ObservableEdges.Remove(x));
                IsModified = true;
            }
            else if (element is IEdge)
            {
                Execute.OnUIThread(() => ObservableEdges.Remove(element as IEdge));
                IsModified = true;
            }
        }

        public void Clear()
        {
            Execute.OnUIThread(() => ObservableEdges.Clear());
            Execute.OnUIThread(() => ObservableVertices.Clear());
            _isModified = false;
            SourceFile = String.Empty;
            SourceFileType = GraphSourceFileType.None;
        }

        public Object Clone()
        {
            return new GraphDocument()
            {
                ObservableVertices = new ObservableCollection<IVertex>(Vertices),
                ObservableEdges = new ObservableCollection<IEdge>(Edges),
                IsDirected = IsDirected,
                IsModified = IsModified,
                IsNotifying = IsNotifying,
                SourceFile = SourceFile,
                SourceFileType = SourceFileType
            };
        }

        public void LoadFrom(string path, GraphSourceFileType fileType)
        {
            switch (fileType)
            {
                case GraphSourceFileType.None:
                    throw new ArgumentException("Type of source file can't be none");
                case GraphSourceFileType.Gxml:
                    GraphReader.FromGxml(path, this);
                    break;
                case GraphSourceFileType.AdjList:
                    GraphReader.FromAdjList(path, this);
                    break;
                case GraphSourceFileType.AdjMatrix:
                    GraphReader.FromAdjMatrix(path, this);
                    break;
                case GraphSourceFileType.EdgesList:
                    GraphReader.FromEdgesList(path, this);
                    break;
                case GraphSourceFileType.IncidenceMatrix:
                    GraphReader.FromIncidenceMatrix(path, this);
                    break;
                default:
                    throw new NotSupportedException($"{fileType.ToString()} not support");
            }

            SourceFile = path;
            SourceFileType = fileType;

            IsModified = false;
        }

        public void SaveTo(string path, GraphSourceFileType fileType)
        {
            switch (fileType)
            {
                case GraphSourceFileType.None:
                    throw new ArgumentException("Type of source file can't be none");
                case GraphSourceFileType.Gxml:
                    GraphWriter.ToGxml(path, this);
                    break;
                default:
                    throw new NotSupportedException($"{fileType.ToString()} not support");
            }
            SourceFile = path;
            SourceFileType = fileType;

            IsModified = false;
        }

        public Dictionary<IVertex, IEnumerable<IVertex>> ToAdjList()
        {
            var adjList = new Dictionary<IVertex, List<IVertex>>();
            foreach (var v in ObservableVertices)
            {
                adjList.Add(v, new List<IVertex>());
            }

            foreach (var e in ObservableEdges)
            {
                adjList[e.Source]?.Add(e.Target);
                if (!e.IsDirected)
                {
                    adjList[e.Target]?.Add(e.Source);
                }
            }

            var res = new Dictionary<IVertex, IEnumerable<IVertex>>();
            foreach (var i in adjList)
            {
                res.Add(i.Key, i.Value);
            }
            return res;
        }

        public IVertex FindVertexByIndex(int index)
        {
            foreach (var i in ObservableVertices)
            {
                if (i.Index == index) return i;
            }
            return null;
        }

        // Private methods
        //
        private int GetNewVertexIndex()
        {
            var newIndex = 1;
            while (FindVertexByIndex(newIndex) != null)
            {
                newIndex++;
            }
            return newIndex;
        }
    }
}
