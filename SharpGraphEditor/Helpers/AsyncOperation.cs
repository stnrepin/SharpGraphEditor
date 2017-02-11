using System;
using System.ComponentModel;

using Caliburn.Micro;

// NOT USED!

namespace SharpGraphEditor.Helpers
{
    public class AsyncOperation
    {
        private BackgroundWorker _worker;
        private readonly System.Action _work;
        private readonly System.Action _onSuccess;
        private readonly Action<Exception> _onFail;

        public AsyncOperation(System.Action work, System.Action onSuccess, Action<Exception> onFail)
        {
            _worker = new BackgroundWorker();

            _work = work ?? throw new ArgumentNullException("work action should not be null");
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public void ExecuteAsync()
        {
            Exception error = null;

            _worker.DoWork += (_, __) =>
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

            _worker.RunWorkerCompleted += (s, e) =>
            {
                if (error == null && _onSuccess != null)
                    _onSuccess?.OnUIThread();

                if (error != null && _onFail != null)
                {
                    Execute.OnUIThread(() => _onFail?.Invoke(error));
                }
            };
            _worker.RunWorkerAsync();
        }

        public void Cancel()
        {
            _worker.CancelAsync();
        }
    }
}
