using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;
using System.ComponentModel;

namespace SharpGraphEditor.ViewModels
{
    public class GraphGeneratorViewModel : PropertyChangedBase
    {
        private static Random _random = new Random();
        private double _dense;
        private int _verticesCount;
        private bool _canGenerate;

        public string ResultEdgesList { get; private set; }

        public GraphGeneratorViewModel(int verticesCount)
        {
            if (!IsNatural(verticesCount))
            {
                verticesCount = 10;
            }

            VerticesCount = verticesCount;
            Dense = 0.5;
            CanGenerate = true;
        }

        public void Generate(IClose closableWindow)
        {
            var edgesList = new StringBuilder();
            var directedEdgesCount = (int)Math.Floor(Dense * VerticesCount * (VerticesCount - 1));
            var edgesCount = directedEdgesCount / 2;

            var allEdges = new List<Tuple<int, int>>();

            for (int i = 1; i <= VerticesCount; i++)
            {
                for (int j = i; j <= VerticesCount; j++)
                {
                    if (i != j)
                    {
                        allEdges.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            var indexes = new List<int>();
            for (int i = 0; i < edgesCount; i++)
            {
                while (true)
                {
                    var index = _random.Next(0, allEdges.Count);
                    if (!indexes.Contains(index))
                    {
                        indexes.Add(index);

                        var sourceIndex = allEdges[index].Item1;
                        var targetIndex = allEdges[index].Item2;

                        edgesList.AppendLine($"{sourceIndex} {targetIndex}");
                        edgesList.AppendLine($"{targetIndex} {sourceIndex}");
                        break;
                    }
                }
            }

            ResultEdgesList = edgesList.ToString();
            closableWindow.TryClose();
        }

        public void Cancel(IClose closableWindow)
        {
            closableWindow.TryClose();
        }

        public double Dense
        {
            get { return _dense; }
            set
            {
                _dense = value;
                NotifyOfPropertyChange(() => Dense);
                CanGenerate = true;
            }
        }

        public int VerticesCount
        {
            get { return _verticesCount; }
            set
            {
                _verticesCount = value;
                NotifyOfPropertyChange(() => VerticesCount);
                CanGenerate = true;
            }
        }

        public bool CanGenerate
        {
            get { return _canGenerate; }
            set
            {
                _canGenerate = value;
                NotifyOfPropertyChange(() => CanGenerate);
            }
        }

        private bool IsNatural(int value)
        {
            return value > 0;
        }
    }
}
