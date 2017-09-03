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
            closableWindow.TryClose(true);
        }

        public void Cancel(IClose closableWindow)
        {
            closableWindow.TryClose(false);
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
