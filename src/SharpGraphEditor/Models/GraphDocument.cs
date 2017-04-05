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
        IncidenceMatrix,
        GraphVizPlainTextExt,
        HierarchicalRtf
    }

    public class GraphDocumentChangedEventArgs : EventArgs
    {

    }

    public class GraphDocument : PropertyChangedBase, IGraph, ICloneable
    {
        // Fields
        //
        private UndoRedoManager _undoRedoManager;

        public ObservableCollection<IVertex> ObservableVertices { get; private set; }
        public ObservableCollection<IEdge> ObservableEdges { get; private set; }

        public IEnumerable<IVertex> Vertices => (ObservableVertices);
        public IEnumerable<IEdge> Edges => (ObservableEdges);

        public event EventHandler<GraphDocumentChangedEventArgs> GraphDocumentChanged;

        public UndoRedoManager UndoRedoManager => _undoRedoManager;

        // Constructor
        //
        public GraphDocument()
        {
            _undoRedoManager = new UndoRedoManager();

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
                var edgesDirectionsStates = ObservableEdges.Select(x => x.IsDirected).ToArray();
                System.Action undo = () =>
                {
                    ObservableEdges.For((x, i) => x.IsDirected = edgesDirectionsStates[i]);
                };
                System.Action redo = () =>
                {
                    ObservableEdges.ForEach(x => x.IsDirected = value);
                };
                _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            }
        }

        // Public methods
        //
        public IVertex AddVertex(IVertex v)
        {
            Execute.OnUIThread(() => ObservableVertices.Add(v));
            return v;
        }

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

            System.Action redo = () =>
            {
                Execute.OnUIThread(() => ObservableVertices.Add(newVertex));
            };
            System.Action undo = () =>
            {
                Remove(newVertex, false);
            };

            _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
            OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            return newVertex;
        }

        public IEdge AddEdge(IEdge e)
        {
            Execute.OnUIThread(() => ObservableEdges.Add(e));
            return e;
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
                System.Action redo = () =>
                {
                    Execute.OnUIThread(() => ObservableEdges.Add(newEdge));
                };
                System.Action undo = () =>
                {
                    Remove(newEdge, false);
                };
                _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
                return newEdge;
            }
            return null;
        }

        public void Remove(IGraphElement element)
        {
            Remove(element, true);
        }

        public void Remove(IGraphElement element, bool useUndoRedo = true)
        {
            if (element is IVertex)
            {
                var vertex = element as IVertex;
                List<IEdge> edges = null;

                System.Action redo = () =>
                {
                    Execute.OnUIThread(() => ObservableVertices.Remove(vertex));
                    edges = ObservableEdges.Where(x => x.Source == vertex || x.Target == vertex)
                                   .ToList();
                    edges.ForEach(x => ObservableEdges.Remove(x));
                };
                System.Action undo = () =>
                {
                    AddVertex(vertex);
                    edges.ForEach(x => AddEdge(x));
                };

                if (useUndoRedo)
                {
                    _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
                }
                else
                {
                    redo();
                }

                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            }
            else if (element is IEdge)
            {
                var edge = element as IEdge;
                System.Action redo = () =>
                {
                    Execute.OnUIThread(() => ObservableEdges.Remove(edge));
                };
                System.Action undo = () =>
                {
                    AddEdge(edge);
                };

                if (useUndoRedo)
                {
                    _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
                }
                else
                {
                    redo();
                }

                OnGraphDocumentChanged(new GraphDocumentChangedEventArgs());
            }
        }

        public void ChangeColor(IVertex vertex, VertexColor newColor)
        {
            var oldColor = vertex.Color;
            System.Action redo = () =>
            {
                vertex.Color = newColor;
            };
            System.Action undo = () =>
            {
                vertex.Color = oldColor;
            };

            _undoRedoManager.AddAndExecute(new SimpleOperation(redo, undo));
        }

        public void Clear()
        {
            Execute.OnUIThread(() => ObservableEdges.Clear());
            Execute.OnUIThread(() => ObservableVertices.Clear());
            _undoRedoManager.Clear();
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


        public IVertex FindVertexByName(string name)
        {
            foreach (var i in ObservableVertices)
            {
                if (i.Name == name)
                    return i;
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
