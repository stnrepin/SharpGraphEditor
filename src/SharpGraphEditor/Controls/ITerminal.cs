using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core;

namespace SharpGraphEditor.Controls
{
    public interface ITerminal : IAlgorithmOutput
    {
        void Clear();
    }
}
