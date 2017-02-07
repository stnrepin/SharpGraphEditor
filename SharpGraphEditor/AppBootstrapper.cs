using System.Windows;

using Caliburn.Micro;

namespace SharpGraphEditor
{
    public class AppBootstrapper : BootstrapperBase
    {
        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ViewModels.MainViewModel>();
        }

        protected override void Configure()
        {
            MessageBinder.SpecialValues.Add("$originalsourcecontext", context =>
            {
                var args = context.EventArgs as RoutedEventArgs;
                var fe = args?.OriginalSource as FrameworkElement;

                return fe?.DataContext;
            });

            base.Configure();
        }
    }
}
