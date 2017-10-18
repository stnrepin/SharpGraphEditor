using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace SharpGraphEditor.Views
{
    public partial class GraphPropertiesView : MetroWindow, IClose
    {
        public GraphPropertiesView()
        {
            InitializeComponent();
        }

        public void TryClose(bool? dialogResult = null)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
