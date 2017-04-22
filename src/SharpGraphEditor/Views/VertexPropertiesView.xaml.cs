using System.Windows;

using MahApps.Metro.Controls;

namespace SharpGraphEditor.Views
{
    public partial class VertexPropertiesView : MetroWindow, Caliburn.Micro.IClose
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
