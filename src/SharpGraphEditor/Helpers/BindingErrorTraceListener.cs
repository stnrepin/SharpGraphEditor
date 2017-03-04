using System;
using System.Diagnostics;

namespace SharpGraphEditor.Helpers
{
    public class BindingErrorTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            NotifyBindingError();
        }

        public override void WriteLine(string message)
        {
            NotifyBindingError();
        }

        private void NotifyBindingError()
        {
            throw new InvalidOperationException("Binding error. Look Output for more details.");
        }
    }
}
