using System.Windows.Controls.Primitives;

namespace SharpGraphEditor.Controls.GraphElements
{
    /// <summary>
    /// Логика взаимодействия для VertexControl.xaml
    /// </summary>
    public partial class VertexControl : Thumb
    {
        public VertexControl()
        {
            InitializeComponent();
            SizeChanged += (_, __) =>
            {
                var centerX = ActualWidth / 2;
                var centerY = ActualHeight / 2;
                Margin = new System.Windows.Thickness(-centerX, -centerY, centerX, centerY);
            };
        }
    }
}
