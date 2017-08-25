using System.Collections;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    public class NewEdge : PropertyChangedBase, IGraphElement, IEnumerable
    {
        private Vertex _source;

        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public bool IsAdding { get; set; } = true;

        public Vertex Target { get; }

        public NewEdge(Vertex start, double mouseX, double mouseY)
        {
            Source = start;
            Target = new Vertex(mouseX, mouseY);
        }

        public Vertex Source
        {
            get { return _source; }
            set
            {
                _source = value;
                NotifyOfPropertyChange(() => Source);
            }
        }

        // In View we use CompositeContainer, which can contains only collections, for displaying Graph and NewEdge.
        // So, we have to present NewEdge as IEnumerable.
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this;
        }
    }
}
