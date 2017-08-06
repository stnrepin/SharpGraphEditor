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
        private List<IOperation> _combinedOperations;

        public SimpleOperation(Action redo, Action undo)
        {
            _redoAction = redo;
            _undoAction = undo;

            _combinedOperations = new List<IOperation>();
        }

        public void Redo()
        {
            _redoAction?.Invoke();
            foreach (var op in _combinedOperations)
            {
                op.Redo();
            }
        }

        public void Undo()
        {
            foreach (var op in _combinedOperations)
            {
                op.Undo();
            }
            _undoAction?.Invoke();
        }

        public void Append(IOperation operation)
        {
            _combinedOperations.Add(operation);
        }
    }
}
