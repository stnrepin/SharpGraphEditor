using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class AlgorithmParameter
    {
        public IAlgorithmOutput Output { get; set; }

        public double MinElementX { get; set; }
        public double MinElementY { get; set; }
        public double MaxElementX { get; set; }
        public double MaxElementY { get; set; }
    }
}
