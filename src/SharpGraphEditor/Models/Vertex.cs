using System;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    public class Vertex : PropertyChangedBase, IVertex
    {
        private double _x;
        private double _y;
        private int _index;
        private string _title;

        public bool IsAdding { get; set; }
        public bool HasPosition { get; set; }

        public Vertex (double x, double y) : this(x, y, -1)
        {
            Title = "";
        }

        public Vertex(double x, double y, int index)
        {
            X = x;
            Y = y;

            Index = index;
            Title = index.ToString();

            IsAdding = false;
            HasPosition = true;
        }

        public double X
        {
            get { return _x; }
            set
            {
                _x = Math.Round(value);
                NotifyOfPropertyChange(() => X);
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = Math.Round(value);
                NotifyOfPropertyChange(() => Y);
            }
        }

        public int Index
        {
            get { return _index; }
            private set
            {
                _index = value;
                NotifyOfPropertyChange(() => Index);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public override string ToString()
        {
            return $"V{Index} - {Title}";
        }
    }
}
