using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    public class AlgorithmResult
    {
        public bool SaveChanges { get; private set; }

        public bool ExecuteStepByStep { get; private set; }

        public AlgorithmResult() : this(false, true)
        {

        }

        public AlgorithmResult(bool saveChanges, bool executeStepByStep)
        {
            SaveChanges = saveChanges;
            ExecuteStepByStep = executeStepByStep;
        }
    }
}
