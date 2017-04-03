using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpGraphEditor.Views
{
    /// <summary>
    /// Interaction logic for VertexPropertiesViewxaml.xaml
    /// </summary>
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
