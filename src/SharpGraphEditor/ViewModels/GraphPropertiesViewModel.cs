using Caliburn.Micro;

using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Models;

namespace SharpGraphEditor.ViewModels
{
    internal class GraphPropertiesViewModel : PropertyChangedBase
    {
        public GraphProperties Properties { get; }

        public GraphPropertiesViewModel(IGraph graph)
        {
            Properties = new GraphProperties(graph);
            Properties.EvaluteAll();
        }

        public void Ok(IClose closableWindow)
        {
            closableWindow.TryClose(true);
        }
    }
}
