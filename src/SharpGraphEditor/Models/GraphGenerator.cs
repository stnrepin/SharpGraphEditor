using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models
{
    public class GraphGenerator
    {
        private readonly static Random _random = new Random();

        public string GenerateEdgesList(double dense, int verticesCount)
        {
            var edgesList = new StringBuilder();
            var directedEdgesCount = (int)Math.Floor(dense * verticesCount * (verticesCount - 1));
            var edgesCount = directedEdgesCount / 2;

            var allEdges = new List<Tuple<int, int>>();

            for (int i = 1; i <= verticesCount; i++)
            {
                for (int j = i; j <= verticesCount; j++)
                {
                    if (i != j)
                    {
                        allEdges.Add(new Tuple<int, int>(i, j));
                    }
                }
            }

            var indexes = new List<int>();
            for (int i = 0; i < edgesCount; i++)
            {
                while (true)
                {
                    var index = _random.Next(0, allEdges.Count);
                    if (!indexes.Contains(index))
                    {
                        indexes.Add(index);

                        var sourceIndex = allEdges[index].Item1;
                        var targetIndex = allEdges[index].Item2;

                        edgesList.AppendLine($"{sourceIndex} {targetIndex}");
                        edgesList.AppendLine($"{targetIndex} {sourceIndex}");
                        break;
                    }
                }
            }

            return edgesList.ToString();
        }
    }
}
