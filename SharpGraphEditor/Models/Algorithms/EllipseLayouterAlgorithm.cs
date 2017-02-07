using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.Algorithms
{
    public class EllipseLayouterAlgorithm : IAlgorithm
    {
        public string Name { get; set; } = "Ellipse layouter";
        public string Description { get; set; } =
            "Has vertices on the boundary of the ellipse";

        public void Run(GraphDocument doc, IDialogsPresenter dialogPresenter)
        {
            var width = doc.MaxElementX;
            var height = doc.MaxElementY;

            var vCount = doc.ObservableVertices.Count;
            if (vCount == 0)
            {
                return;
            }
            else if (vCount == 1)
            {
                var v = doc.ObservableVertices[0];
                v.X = doc.MaxElementX / 2;
                v.Y = doc.MaxElementY / 2;
                return;
            }

            var minWidth = doc.MinElementX;
            var minHeight = doc.MinElementY;

            // 15 - радиус окружности. Берем половину.
            width -= minWidth * 2.25;
            height -= minHeight * 2.25;

            var centerX = width / 2;
            var centerY = height / 2;

            for (int i = 0; i < vCount; i++)
            {
                var v = doc.ObservableVertices[i];
                v.X = Math.Ceiling(centerX + minWidth + centerX * Math.Cos(Math.PI * i / vCount * 2));
                v.Y = Math.Ceiling(centerY + minHeight + centerY * Math.Sin(Math.PI * i / vCount * 2));
            }
        }
    }
}
