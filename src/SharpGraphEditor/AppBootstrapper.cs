using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

using Caliburn.Micro;

using SharpGraphEditor.Services;

namespace SharpGraphEditor
{
    public class AppBootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;
        private IDialogsPresenter _dialogsPresenter;

        public AppBootstrapper()
        {
            _container = new SimpleContainer();
            _dialogsPresenter = new WindowDialogsPresenter();

            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ViewModels.MainViewModel>();
        }

        protected override void Configure()
        {
            _container.Singleton<IDialogsPresenter, WindowDialogsPresenter>();
            _container.Singleton<IWindowManager, WindowManager>();

            _container.PerRequest<ViewModels.MainViewModel>();

            MessageBinder.SpecialValues.Add("$originalsourcecontext", context =>
            {
                var args = context.EventArgs as RoutedEventArgs;
                var fe = args?.OriginalSource as FrameworkElement;

                return fe?.DataContext;
            });

            base.Configure();
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var instance = _container.GetInstance(serviceType, key);
            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
