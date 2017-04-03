using System;
using System.Diagnostics;
using System.Windows;

using Caliburn.Micro;

namespace SharpGraphEditor.Views
{
    public partial class AboutView : Window, IClose
    {
        public AboutView()
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
