using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core
{
    public interface IAlgorithmOutput
    {
        void Write(string text);
        void WriteLine();
        void WriteLine(string text);
        string ReadLine();
    }
}
