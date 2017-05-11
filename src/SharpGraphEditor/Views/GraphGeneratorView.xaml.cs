using System;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using System.Windows;

namespace SharpGraphEditor.Views
{
    /// <summary>
    /// Interaction logic for GraphGeneratorView.xaml
    /// </summary>
    public partial class GraphGeneratorView : MetroWindow, IClose
    {
        public GraphGeneratorView()
        {
            InitializeComponent();
        }

        public void TryClose(bool? dialogResult = default(bool?))
        {
            DialogResult = dialogResult;
            Close();
        }

        private void ValidationError(object sender, ValidationErrorEventArgs e)
        {
            var vm = DataContext as ViewModels.GraphGeneratorViewModel;
            if (vm == null)
            {
                return;
            }

            vm.CanGenerate = false;
        }
    }
}
