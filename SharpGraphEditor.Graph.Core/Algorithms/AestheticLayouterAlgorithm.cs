using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Graph.Core.Algorithms
{
    // Use this:
    //    https://github.com/effect/graph_editor/blob/master/org/amse/fedotov/graph_editor/layouters/impl/AestheticLayouter.java


    public class AestheticLayouterAlgorithm : IAlgorithm
    {
        private struct Point
        {
            public double X { get; set; }
            public double Y { get; set; }

            public Point(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double SqDistanceTo(Point p)
            {
                return Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2);
            }
        }

        private static readonly Random Rand = new Random();

        private readonly double Delta = 0.0001;
        private readonly double IterationsCount = 400;
        private readonly double Epsilon = 0.0005;
        private readonly Point ZeroPoint = new Point(0, 0);

        private double DistanceBetweenVertices;

        public String Name { get; } = "Aesthetic layouter";
        public String Description { get; } = "Structures graph";

        public void Run(IGraph graph, AlgorithmParameter p)
        {
            var minWidth = p.MinElementX;
            var minHeight = p.MinElementY;
            var width = p.MaxElementX - minWidth * 2.25;
            var height = p.MaxElementY - minHeight * 2.25;

            var verticesCount = graph.Vertices.Count();

            DistanceBetweenVertices = 0.75 * Math.Sqrt(width * height / verticesCount);

            var forces = new Point[verticesCount];
            for (int iteration = 0; iteration < IterationsCount; iteration++)
            {
                if (iteration % 10 == 0)
                {
                    Normalize(graph, width, height);
                    int shift = 20;
                    var r = new Random(239111);
                    foreach (var v in graph.Vertices)
                    {
                        v.X += Rand.Next(shift) - shift / 2;
                        v.Y += Rand.Next(shift) - shift / 2;
                    }
                }
                for (int i = 0; i < verticesCount; i++)
                {
                    var vertex = graph.Vertices.ElementAt(i);
                    forces[i] = new Point(0, 0);
                    for (int j = 0; j < verticesCount; j++)
                    {
                        if (i != j)
                        {
                            var v = graph.Vertices.ElementAt(j);
                            var curForce = SomeMethod1(v, vertex);
                            forces[i].X += curForce.X;
                            forces[i].Y += curForce.Y;
                        }
                    }
                    foreach (var v in GetAdjVerticesFor(graph, vertex))
                    {
                        var curForce = SomeMethod2(v, vertex);
                        forces[i].X += curForce.X;
                        forces[i].Y += curForce.Y;
                    }
                }
                for (int i = 0; i < verticesCount; i++)
                {
                    var v = graph.Vertices.ElementAt(i);
                    v.X += forces[i].X * Delta;
                    v.Y += forces[i].Y * Delta;
                }
            }

            Normalize(graph, width, height);
            foreach(var v in graph.Vertices)
            {
                v.X += p.MinElementX;
                v.Y += p.MinElementY;
            }
        }

        //coulompRepulsion
        private Point SomeMethod1(IVertex u, IVertex v)
        {
            var uv = new Point(v.X - u.X, v.Y - u.Y);
            var koef = DistanceBetweenVertices * DistanceBetweenVertices / (Epsilon + uv.SqDistanceTo(ZeroPoint));
            return new Point(koef * uv.X, koef * uv.Y);
        }

        //hookeAttraction
        private Point SomeMethod2(IVertex u, IVertex v)
        {
            var uv = new Point(v.X - u.X, v.Y - u.Y);
            var koef = -uv.SqDistanceTo(ZeroPoint) / (Epsilon + DistanceBetweenVertices);
            return new Point(koef * uv.X, koef * uv.Y);
        }

        private IEnumerable<IVertex> GetAdjVerticesFor(IGraph graph, IVertex vertex)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == vertex)
                {
                    yield return edge.Target;
                }
                else if (edge.Target == vertex)
                {
                    yield return edge.Source;
                }
            }
        }

        private void Normalize(IGraph graph, double width, double height)
        {
            var minX = Double.MaxValue;
            var minY = Double.MaxValue;

            foreach (var v in graph.Vertices)
            {
                minX = Math.Min(minX, v.X);
                minY = Math.Min(minY, v.Y);
            }

            foreach (var v in graph.Vertices)
            {
                v.X -= minX;
                v.Y -= minY;
            }

            var maxX = Double.MinValue;
            var maxY = Double.MinValue;
            foreach (var v in graph.Vertices)
            {
                maxX = Math.Max(maxX, v.X);
                maxY = Math.Max(maxY, v.Y);
            }

            foreach (var v in graph.Vertices)
            {

                if (maxX > 0)
                {
                    v.X *= width / maxX;
                }
                if (maxY > 0)
                {
                    v.Y *= height / maxY;
                }
            }
        }


    }
}
