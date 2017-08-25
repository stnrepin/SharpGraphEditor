using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpGraphEditor.Controls
{
    public class NewEdgeShape : Shape
    {
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(NewEdgeShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(NewEdgeShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(NewEdgeShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(NewEdgeShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(double), typeof(NewEdgeShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double X1
        {
            get { return (double)base.GetValue(X1Property); }
            set { base.SetValue(X1Property, value); }
        }

        public double Y1
        {
            get { return (double)base.GetValue(Y1Property); }
            set { base.SetValue(Y1Property, value); }
        }

        public double X2
        {
            get { return (double)base.GetValue(X2Property); }
            set { base.SetValue(X2Property, value); }
        }

        public double Y2
        {
            get { return (double)base.GetValue(Y2Property); }
            set { base.SetValue(Y2Property, value); }
        }

        public double Radius
        {
            get { return (double)base.GetValue(RadiusProperty); }
            set { base.SetValue(RadiusProperty, value); }
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                var geometry = new StreamGeometry()
                {
                    FillRule = FillRule.EvenOdd
                };
                using (StreamGeometryContext context = geometry.Open())
                {
                    DrawLine(context);
                    DrawEllipse(context);
                }

                geometry.Freeze();

                return geometry;
            }
        }

        private void DrawLine(StreamGeometryContext context)
        {
            var theta = Math.Atan2(Y1 - Y2, X1 - X2);
            var sint = Math.Sin(theta);
            var cost = Math.Cos(theta);

            var x2WithOffset = X2 + Radius * cost;
            var y2WithOffset = Y2 + Radius * sint;

            var pt1 = new Point(X1, Y1);
            var pt2 = new Point(x2WithOffset, y2WithOffset);

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
        }

        private void DrawEllipse(StreamGeometryContext context)
        {
            // Do not use because it is in DrawLine(...)
            // context.BeginFigure(...);

            double controlPointRatio = (Math.Sqrt(2) - 1) * 4 / 3;

            var x0 = X2 - Radius;
            var x1 = X2 - Radius * controlPointRatio;
            var x2 = X2;
            var x3 = X2 + Radius * controlPointRatio;
            var x4 = X2 + Radius;

            var y0 = Y2 - Radius;
            var y1 = Y2 - Radius * controlPointRatio;
            var y2 = Y2;
            var y3 = Y2 + Radius * controlPointRatio;
            var y4 = Y2 + Radius;

            context.BeginFigure(new Point(x2, y0), false, true);
            context.BezierTo(new Point(x3, y0), new Point(x4, y1), new Point(x4, y2), true, true);
            context.BezierTo(new Point(x4, y3), new Point(x3, y4), new Point(x2, y4), true, true);
            context.BezierTo(new Point(x1, y4), new Point(x0, y3), new Point(x0, y2), true, true);
            context.BezierTo(new Point(x0, y1), new Point(x1, y0), new Point(x2, y0), true, true);
        }
    }
}
