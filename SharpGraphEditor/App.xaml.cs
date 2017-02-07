using System.Diagnostics;
using System.Windows;

namespace SharpGraphEditor
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            PresentationTraceSources.DataBindingSource.Listeners.Add(new Helpers.BindingErrorTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
        }
    }
}
