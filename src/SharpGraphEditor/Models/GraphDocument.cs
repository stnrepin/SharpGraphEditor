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
    public enum GraphSourceType
    {
        None,
        Gxml,
        AdjList,
        AdjMatrix,
        EdgesList,
        IncidenceMatrix
    }

    public class GraphDocumentChangedEventArgs : EventArgs
    {

    }

    public class GraphDocument : PropertyChangedBase, IGraph, ICloneable
    {
        // Fields
        //
        public ObservableCollection<IVertex> ObservableVertices { get; private set; }
        public ObservableCollection<IEdge> ObservableEdges { get; private set; }

        public IEnumerable<IVertex> Vertices => (ObservableVertices);
        public IEnumerable<IEdge> Edges => (ObservableEdges);

        public event EventHandler<GraphDocumentChangedEventArgs> GraphDocumentChanged;

        // Constructor
        //
        public GraphDocument()
        {
            ObservableVertices = new ObservableCollection<IVertex>();
            ObservableEdges = new ObservableCollection<IEdge>();
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
            set
            {
                ObservableEdges.ForEach(x => x.IsDirected = value);
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
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
            OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            return newVertex;
        }

        public IEdge AddEdge(IVertex source, IVertex target, bool makeNotDirectedIfreversedExisted = false)
        {
            return AddEdge(source, target, IsDirected, makeNotDirectedIfreversedExisted);
        }

        public IEdge AddEdge(IVertex source, IVertex target, bool isDirected, bool makeNotDirectedIfreversedExisted = false)
        {
            if (source == target) return null;

            var existingEdge = ObservableEdges.FirstOrDefault(x => x.Source == source && x.Target == target);

            if (existingEdge == null)
            {
                var reversedEdge = ObservableEdges.FirstOrDefault(x => x.Source == target && x.Target == source);
                if (reversedEdge != null)
                {
                    if (makeNotDirectedIfreversedExisted && reversedEdge.IsDirected)
                    {
                        reversedEdge.IsDirected = false;
                        return reversedEdge;
                    }
                    return reversedEdge;
                }

                var newEdge = new Edge(source, target, isDirected);
                Execute.OnUIThread(() => ObservableEdges.Add(newEdge));
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
                return newEdge;
            }
            return null;
        }

        public void Remove(IGraphElement element)
        {
            if (element is IVertex)
            {
                var vertex = element as IVertex;

                Execute.OnUIThread(() => ObservableVertices.Remove(vertex));
                ObservableEdges.Where(x => x.Source == vertex || x.Target == vertex)
                               .ToList()
                               .ForEach(x => ObservableEdges.Remove(x));
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            }
            else if (element is IEdge)
            {
                Execute.OnUIThread(() => ObservableEdges.Remove(element as IEdge));
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            }
        }

        public void Clear()
        {
            Execute.OnUIThread(() => ObservableEdges.Clear());
            Execute.OnUIThread(() => ObservableVertices.Clear());
        }

        public Object Clone()
        {
            return new GraphDocument()
            {
                ObservableVertices = new ObservableCollection<IVertex>(Vertices),
                ObservableEdges = new ObservableCollection<IEdge>(Edges),
                IsDirected = IsDirected,
                IsNotifying = IsNotifying,
            };
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

        private void OnGraphDocumentChanged(GraphDocumentChangedEventArgs e)
        {
            GraphDocumentChanged?.Invoke(this, e);
        }
    }
}
