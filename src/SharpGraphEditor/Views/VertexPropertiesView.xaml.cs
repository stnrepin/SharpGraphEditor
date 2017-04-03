using System.Windows;

namespace SharpGraphEditor.Views
{
    public partial class VertexPropertiesView : Window, Caliburn.Micro.IClose
    {
        public VertexPropertiesView()
        {
            InitializeComponent();
        }

        public void TryClose(bool? dialogResult = default(bool?))
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
