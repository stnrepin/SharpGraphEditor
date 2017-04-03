using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Elements;

using Caliburn.Micro;


namespace SharpGraphEditor.ViewModels
{
    public class VertexPropertiesViewModel : PropertyChangedBase
    {
        private IVertex _targetVertex;

        public VertexPropertiesViewModel(IVertex vertex)
        {
            _targetVertex = vertex;
        }

        public void Close(IClose closeableWindow)
        {
            closeableWindow?.TryClose(true);
        }

        public IVertex TargetVertex
        {
            get { return _targetVertex; }
            set
            {
                _targetVertex = value;
                NotifyOfPropertyChange(() => TargetVertex);
            }
        }
    }
}
