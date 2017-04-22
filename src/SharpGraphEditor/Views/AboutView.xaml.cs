using System;
using System.Diagnostics;
using System.Windows;

using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace SharpGraphEditor.Views
{
    public partial class AboutView : MetroWindow, IClose
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
