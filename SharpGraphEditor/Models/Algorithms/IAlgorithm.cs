using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.Algorithms
{
    public interface IAlgorithm
    {
        string Name { get; set; }
        string Description { get; set; }

        void Run(GraphDocument doc, IDialogsPresenter dialogPresenter);
    }
}
