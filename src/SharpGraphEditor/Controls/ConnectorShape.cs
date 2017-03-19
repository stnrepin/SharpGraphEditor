using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SharpGraphEditor.Controls
{
    // Source:
    //     http://blogs.microsoft.co.il/tomershamam/2008/01/23/wpf-arrow-and-custom-shapes/

    public sealed class ConnectorShape : Shape
	{
		public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register("X2", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadWidthProperty = 
            DependencyProperty.Register("HeadWidth", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty HeadHeightProperty =
            DependencyProperty.Register("HeadHeight", typeof(double), typeof(ConnectorShape), 
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender 
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty EndPositionOffsetProperty =
            DependencyProperty.Register("EndPositionOffset", typeof(double), typeof(ConnectorShape),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty IsDirectedProperty =
            DependencyProperty.Register("IsDirected", typeof(bool), typeof(ConnectorShape),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender
                    | FrameworkPropertyMetadataOptions.AffectsMeasure));


        [TypeConverter(typeof(LengthConverter))]
		public double X1
		{
			get { return (double)base.GetValue(X1Property); }
			set { base.SetValue(X1Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Y1
		{
			get { return (double)base.GetValue(Y1Property); }
			set { base.SetValue(Y1Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double X2
		{
			get { return (double)base.GetValue(X2Property); }
			set { base.SetValue(X2Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double Y2
		{
			get { return (double)base.GetValue(Y2Property); }
			set { base.SetValue(Y2Property, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double HeadWidth
		{
			get { return (double)base.GetValue(HeadWidthProperty); }
			set { base.SetValue(HeadWidthProperty, value); }
		}

		[TypeConverter(typeof(LengthConverter))]
		public double HeadHeight
		{
			get { return (double)base.GetValue(HeadHeightProperty); }
			set { base.SetValue(HeadHeightProperty, value); }
		}

        [TypeConverter(typeof(LengthConverter))]
        public double EndPositionOffset
        {
            get { return (double)base.GetValue(EndPositionOffsetProperty); }
            set { base.SetValue(EndPositionOffsetProperty, value); }
        }

        public bool IsDirected
        {
            get { return (bool)base.GetValue(IsDirectedProperty); }
            set { base.SetValue(IsDirectedProperty, value); }
        }


        protected override Geometry DefiningGeometry
		{
			get
			{
				var geometry = new StreamGeometry();
				geometry.FillRule = FillRule.EvenOdd;

				using (StreamGeometryContext context = geometry.Open())
				{
					InternalDrawArrowGeometry(context);
				}

				geometry.Freeze();

				return geometry;
			}
		}

		private void InternalDrawArrowGeometry(StreamGeometryContext context)
		{
            var theta = Math.Atan2(Y1 - Y2, X1 - X2);
			var sint = Math.Sin(theta);
			var cost = Math.Cos(theta);

            var x2WithOffset = X2 + EndPositionOffset * cost;
            var y2WithOffset = Y2 + EndPositionOffset * sint;

            var pt1 = new Point(X1, Y1);
			var pt2 = new Point(x2WithOffset, y2WithOffset);

            var pt3 = new Point(
                x2WithOffset + (HeadWidth * cost - HeadHeight * sint),
                y2WithOffset + (HeadWidth * sint + HeadHeight * cost));

			var pt4 = new Point(
                x2WithOffset + (HeadWidth * cost + HeadHeight * sint),
                y2WithOffset - (HeadHeight * cost - HeadWidth * sint));

            context.BeginFigure(pt1, true, false);
			context.LineTo(pt2, true, true);
            if (IsDirected) context.PolyLineTo(new[] { pt3, pt4, pt2 }, true, true);
        }
	}
}
