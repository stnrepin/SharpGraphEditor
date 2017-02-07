using Caliburn.Micro;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    public class Edge : PropertyChangedBase, IEdge
    {
        private IVertex _source;
        private IVertex _target;
        private bool _isAdding;
        private bool _isDirected;

        public double X { get; set; }
        public double Y { get; set; }

        public Edge(IVertex start, IVertex end, bool isDirected)
        {
            Source = start;
            Target = end;
            IsDirected = isDirected;
        }

        public IVertex Source
        {
            get { return _source; }
            set
            {
                _source = value;
                NotifyOfPropertyChange(() => Source);
            }
        }

        public IVertex Target
        {
            get { return _target; }
            set
            {
                _target = value;
                NotifyOfPropertyChange(() => Target);
            }
        }

        public bool IsAdding
        {
            get { return _isAdding; }
            set
            {
                _isAdding = value;
                NotifyOfPropertyChange(() => IsAdding);
            }
        }

        public bool IsDirected
        {
            get { return _isDirected; }
            set
            {
                _isDirected = value;
                NotifyOfPropertyChange(() => IsDirected);
            }
        }

        public override string ToString()
        {
            return $"{Source.Title}->{Target.Title}";
        }
    }
}
