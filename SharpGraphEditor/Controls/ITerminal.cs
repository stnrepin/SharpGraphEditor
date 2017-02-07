using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Controls
{
    public interface ITerminal
    {
        void Clear();

        void Write(string text);

        void WriteLine();

        void WriteLine(string text);

        string ReadLine();
    }
}
