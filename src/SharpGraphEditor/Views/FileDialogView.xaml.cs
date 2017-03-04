using System.Windows;

using Caliburn.Micro;

namespace SharpGraphEditor.Views
{
    /// <summary>
    /// Interaction logic for FileDialogView.xaml
    /// </summary>
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
