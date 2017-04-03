using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using SharpGraphEditor.ViewModels;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Views
{
    public partial class MainView : Window
    {
        public MainViewModel Vm => (DataContext as MainViewModel);

        public MainView()
        {
            InitializeComponent();

            Loaded += (sender, e) =>
            {
                Vm.Terminal = OutputTerminal;
                Vm.ViewLoaded();
            };
        }

        private void MainWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Vm.NewEdge != null)
            {
                var pos = e.GetPosition(GraphControl);
                Vm.NewEdge.Target.X = pos.X;
                Vm.NewEdge.Target.Y = pos.Y;
            }
        }

        private void GraphControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePosition = e.GetPosition(GraphControl);
            Vm.CanvasClick(Math.Round(mousePosition.X), Math.Round(mousePosition.Y), GetGraphElementUnderMouse());
        }

        private void GraphControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Vm.NewEdge = null;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Vm.Exit();
            e.Cancel = true;
        }

        private void GraphElement_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Vm.CurrentCursorMode != Models.CursorMode.Default)
            {
                return;
            }

            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            var graphElement = thumb.DataContext as IGraphElement;
            if (graphElement == null)
                return;

            graphElement.X += e.HorizontalChange;
            graphElement.Y += e.VerticalChange;

            Vm.SelectedElement = graphElement;
            if (thumb.TemplatedParent != null)
                Canvas.SetZIndex(thumb.TemplatedParent as UIElement, 1);
        }

        private void GraphElement_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var thumb = sender as Thumb;
            if (thumb == null)
                return;

            if (thumb.TemplatedParent != null)
                Canvas.SetZIndex(thumb.TemplatedParent as UIElement, 0);
        }

        private void GraphControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                return;

            if (e.Delta > 0)
            {
                Vm.ChangeZoomByPercents(10);
            } 
            else if (e.Delta < 0)
            {
                Vm.ChangeZoomByPercents(-10);
            }
        }

        private void GraphControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var size = VisualTreeHelper.GetDescendantBounds(GraphControl);
            Vm.AlgorithmParameter.MaxElementX = size.Width;
            Vm.AlgorithmParameter.MaxElementY = size.Height;
        }

        private IGraphElement GetGraphElementUnderMouse()
        {
            return (Mouse.DirectlyOver as FrameworkElement)?.DataContext as IGraphElement;
        }
    }
}
