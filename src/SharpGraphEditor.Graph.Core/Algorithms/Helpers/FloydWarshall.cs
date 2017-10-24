using System;

namespace SharpGraphEditor.Graph.Core.Algorithms.Helpers
{
    public class FloydWarshall
    {
        public const int MaxDistance = 99999;

        public int[,] Run(IGraph graph, int verticesCount)
        {
            var graphMatrix = FormatStorage.AdjMatrixFormatStorage.ToAdjMatrix(graph);

            int[,] distance = new int[verticesCount, verticesCount];

            for (int i = 0; i < verticesCount; i++)
            {
                for (int j = 0; j < verticesCount; j++)
                {
                    distance[i, j] = graphMatrix[i, j];
                    if (distance[i, j] == 0)
                    {
                        distance[i, j] = MaxDistance;
                    }
                }
            }

            for (int k = 0; k < verticesCount; k++)
            {
                for (int i = 0; i < verticesCount; i++)
                {
                    for (int j = 0; j < verticesCount; j++)
                    {
                        distance[i, j] = Math.Min(distance[i, j], distance[i, k] + distance[k, j]);
                    }
                }
            }
            return distance;
        }
    }
}
