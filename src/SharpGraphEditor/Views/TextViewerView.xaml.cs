using System.Windows;

using Caliburn.Micro;

namespace SharpGraphEditor.Views
{
    public partial class TextViewerView : Window, IClose
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
