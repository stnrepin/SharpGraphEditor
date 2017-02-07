using System;
using System.ComponentModel;

using Caliburn.Micro;


// NOT USED!

namespace SharpGraphEditor.Helpers
{
    public class AsyncOperation : IResult
    {
        private readonly System.Action _work;
        private readonly System.Action _onSuccess;
        private readonly Action<Exception> _onFail;

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public AsyncOperation(System.Action work, System.Action onSuccess, Action<Exception> onFail)
        {
            //_work = work ?? throw new ArgumentNullException("work action should not be null");
            _work = work;
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public void Execute(CoroutineExecutionContext context)
        {
            Exception error = null;
            var worker = new BackgroundWorker();

            worker.DoWork += (_, __) =>
            {
                try
                {
                    _work();
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            };

            worker.RunWorkerCompleted += (s, e) =>
            {
                if (error == null && _onSuccess != null)
                    _onSuccess?.OnUIThread();

                if (error != null && _onFail != null)
                {
                    Caliburn.Micro.Execute.OnUIThread(() => _onFail?.Invoke(error));
                }

                Completed(this, new ResultCompletionEventArgs { Error = error });
            };
            worker.RunWorkerAsync();
        }
    }
}
