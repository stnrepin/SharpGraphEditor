using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    internal class GraphProperties : PropertyChangedBase
    {
        private IGraph _graph;
        private int _verticesCount;
        private int[,] _distances;

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
            var fw = new Graph.Core.Algorithms.Helpers.FloydWarshall();
            _distances = fw.Run(_graph, _verticesCount);

            EvaluteDense();
            EvaluteEccentricity();
            EvaluteRadiusAndDiameter();
        }

        private void EvaluteRadiusAndDiameter()
        {
            foreach(var v in _graph.Vertices)
            {
                Radius = Math.Min(Radius, Eccentricity[v.Index - 1].Item2);
                Diameter = Math.Max(Diameter, Eccentricity[v.Index - 1].Item2);
            }
        }

        private void EvaluteEccentricity()
        {
            var eccentricities = new List<Tuple<IVertex, int>>();
            foreach(var v in _graph.Vertices)
            {
                var i = v.Index  - 1;
                var currentEccentricity = 0;
                for (int j = 0; j < _verticesCount; j++)
                {
                    currentEccentricity = Math.Max(currentEccentricity, _distances[i, j]);
                }
                eccentricities.Add(Tuple.Create(v, currentEccentricity == Graph.Core.Algorithms.Helpers.FloydWarshall.MaxDistance ? 0 : currentEccentricity));
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
