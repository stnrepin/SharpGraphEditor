using System.Windows;

using Caliburn.Micro;

namespace SharpGraphEditor.Views
{
    public partial class FileDialogView : Window, IClose
    {
        public FileDialogView()
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
