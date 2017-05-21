using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public interface IAlgorithmHost
    {
        IAlgorithmOutput Output { get; set; }

        double MinElementX { get; set; }
        double MinElementY { get; set; }
        double MaxElementX { get; set; }
        double MaxElementY { get; set; }

        IVertex GetSelectedVertex();
    }
}
