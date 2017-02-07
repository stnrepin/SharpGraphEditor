using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models
{
    public interface IGraphElement
    {
        double X { get; set; }

        double Y { get; set; }

        bool IsAdding { get; set; }
    }
}
