using System.Windows;

using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace SharpGraphEditor.Views
{
    public partial class TextViewerView : MetroWindow, IClose
    {
        public TextViewerView()
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
