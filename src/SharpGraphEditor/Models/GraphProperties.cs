using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.Algorithms.Helpers;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    internal class GraphProperties : PropertyChangedBase
    {
        private IGraph _graph;
        private int _verticesCount;

        public int Radius { get; private set; }
        public int Diameter { get; private set; }
        public double Dense { get; private set; }
        public ObservableCollection<Tuple<IVertex, int>> Eccentricity { get; private set; }

        public GraphProperties(IGraph graph)
        {
            _graph = graph;
            _verticesCount = graph.Vertices.Count();
            Eccentricity = new ObservableCollection<Tuple<IVertex, int>>();
        }

        public void EvaluteAll()
        {
            EvaluteDense();
            EvaluteEccentricity();
            EvaluteRadiusAndDiameter();
        }

        private void EvaluteRadiusAndDiameter()
        {
            foreach(var v in _graph.Vertices)
            {
                Radius = Eccentricity.Max(x => x.Item2);
                Diameter = Eccentricity.Min(x => x.Item2);
            }
        }

        private void EvaluteEccentricity()
        {
            var eccentricities = new List<Tuple<IVertex, int>>();

            var adjList = _graph.ToAdjList();
            var djAlgorithm = new DijkstraAlgorithm(_graph, adjList);

            foreach (var pair in adjList)
            {
                var v = pair.Key;
                var neighbors = pair.Value;

                var maxEccentricity = djAlgorithm.GetShortestPaths(v).Where(x => x.Value != DijkstraAlgorithm.MaxDistance)
                                                                    ?.Max(y => y.Value);
                if (maxEccentricity.HasValue)
                {
                    var meValue = maxEccentricity.Value;
                    eccentricities.Add(Tuple.Create(v, meValue == DijkstraAlgorithm.MaxDistance ? 0 : meValue));
                }
            }
            Eccentricity = new ObservableCollection<Tuple<IVertex, int>>(eccentricities.OrderBy(x => x.Item1.Index));
        }

        private void EvaluteDense()
        {
            var edgesCount = _graph.Edges.Count();
            var verticesCount = _verticesCount;

            if (verticesCount < 2)
            {
                Dense = 0;
                return;
            }

            Dense = Math.Round((double)(2 * edgesCount) / (verticesCount * (verticesCount - 1)), 3);
            NotifyOfPropertyChange(nameof(Dense));
        }
    }
}
