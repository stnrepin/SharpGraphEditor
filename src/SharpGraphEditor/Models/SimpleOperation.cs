using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Models
{
    public class SimpleOperation : IOperation
    {
        private Action _redoAction;
        private Action _undoAction;

        public SimpleOperation(Action redo, Action undo)
        {
            _redoAction = redo;
            _undoAction = undo;
        }

        public void Redo()
        {
            _redoAction?.Invoke();
        }

        public void Undo()
        {
            _undoAction?.Invoke();
        }
    }
}
