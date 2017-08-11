using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models
{
    public enum CursorMode
    {
        Default,
        Add,
        Remove
    }

    public class CursorModeManager
    {
        public CursorMode Current { get; private set; }

        public CursorModeManager()
        {
            Current = CursorMode.Default;
        }

        public void Change(CursorMode newMode)
        {
            Current = newMode;
        }

        public void Change(string newModeName)
        {
            if (Enum.TryParse(newModeName, out CursorMode newValue))
            {
                Current = newValue;
                return;
            }
            throw new ArgumentException("incorrect name of CursorMode");
        }
    }
}
