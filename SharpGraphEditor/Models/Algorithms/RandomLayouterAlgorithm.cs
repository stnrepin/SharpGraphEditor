using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.Algorithms
{
    public class RandomLayouterAlgorithm : IAlgorithm
    {
        private static Random _random = new Random();

        public string Name { get; set; } =
                        "Random layouter";
        public string Description { get; set; } =
                        "Randomize vertices position";

        public void Run(GraphDocument doc, IDialogsPresenter dialogPresenter)
        {
            foreach (var v in doc.ObservableVertices)
            {
                v.X = _random.Next((int)doc.MinElementX, (int)(doc.MaxElementX - doc.MinElementX));
                v.Y = _random.Next((int)doc.MinElementY, (int)(doc.MaxElementY - doc.MinElementY));
            }
        }
    }
}
