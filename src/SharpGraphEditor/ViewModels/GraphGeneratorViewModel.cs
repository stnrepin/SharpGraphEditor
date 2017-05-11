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
        private int[] _vertices;
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
            _vertices = Enumerable.Range(1, VerticesCount).ToArray();

            var edgesList = new StringBuilder();
            var directedEdgesCount = (int)Math.Floor(Dense * VerticesCount * (VerticesCount - 1));
            var edgesCount = directedEdgesCount / 2;

            for (int i = 0; i < edgesCount; i++)
            {
                var sourceIndex = _vertices[_random.Next(0, VerticesCount)];
                var targetIndex = _vertices[_random.Next(0, VerticesCount)];

                edgesList.AppendLine($"{sourceIndex} {targetIndex}");
                edgesList.AppendLine($"{targetIndex} {sourceIndex}");
            }

            ResultEdgesList = edgesList.ToString();
            closableWindow.TryClose();
        }

        public double Dense
        {
            get { return _dense; }
            set
            {
                System.Diagnostics.Debug.WriteLine(value);
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
