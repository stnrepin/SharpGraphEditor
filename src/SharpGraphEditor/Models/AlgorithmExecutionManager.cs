using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpGraphEditor.Graph.Core;
using SharpGraphEditor.Graph.Core.Algorithms;
using SharpGraphEditor.Graph.Core.Elements;

namespace SharpGraphEditor.Models
{
    public class AlgorithmExecutionManager : Caliburn.Micro.PropertyChangedBase
    {
        private CancellationTokenSource _algorithmTaskCancellationTokenSource;
        private bool _isAlgorithmExecuting;
        private int _undoRedoStartPosition;
        private Task<bool> _algorithmTask;

        public int StepInterval { get; set; }

        public AlgorithmExecutionManager()
        {
            _undoRedoStartPosition = UndoRedoManager.Instance.Position;
            IsAlgorithmExecuting = false;
            _algorithmTaskCancellationTokenSource = new CancellationTokenSource();

            StepInterval = 700;
        }

        public bool IsAlgorithmExecuting
        {
            get { return _isAlgorithmExecuting; }
            set
            {
                if (_isAlgorithmExecuting != value)
                {
                    _isAlgorithmExecuting = value;
                    NotifyOfPropertyChange(() => IsAlgorithmExecuting);
                }
            }
        }

        public void Restart()
        {
            IsAlgorithmExecuting = true;
            while (UndoRedoManager.Instance.Position > _undoRedoStartPosition)
            {

                UndoRedoManager.Instance.Undo();
            }
            IsAlgorithmExecuting = false;
        }

        public void StepNext()
        {
            IsAlgorithmExecuting = true;
            UndoRedoManager.Instance.Redo();
            IsAlgorithmExecuting = false;
        }

        public void StepBack()
        {
            IsAlgorithmExecuting = true;
            
            if (UndoRedoManager.Instance.Position > _undoRedoStartPosition)
            {
                UndoRedoManager.Instance.Undo();
            }
            IsAlgorithmExecuting = false;
        }

        public void Stop(bool saveChanges)
        {
            if (!saveChanges)
            {
                Restart();
                UndoRedoManager.Instance.CutOff();
                _undoRedoStartPosition = -1;
            }
            _algorithmTaskCancellationTokenSource.Cancel();
            IsAlgorithmExecuting = false;
        }

        public void ContinueOrPause()
        {
            if (IsAlgorithmExecuting)
            {
                _algorithmTaskCancellationTokenSource.Cancel();
                IsAlgorithmExecuting = false;
            }
            else
            {
                IsAlgorithmExecuting = true;
                _algorithmTaskCancellationTokenSource = new CancellationTokenSource();
                var task = Task.Factory.StartNew(() =>
                {
                    while (UndoRedoManager.Instance.CanRedo)
                    {
                        if (_algorithmTaskCancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        UndoRedoManager.Instance.Redo();
                        Thread.Sleep(StepInterval);
                    }
                    IsAlgorithmExecuting = false;
                }, _algorithmTaskCancellationTokenSource.Token);
            }
        }

        public Task<bool> Run(IAlgorithm algorithm, IGraph graph, IAlgorithmHost host)
        {
            _algorithmTask = Task.Factory.StartNew<bool>(() =>
            {
                IsAlgorithmExecuting = true;
                var res = algorithm?.Run(graph, host);

                if (!res.ExecuteStepByStep || _undoRedoStartPosition == UndoRedoManager.Instance.Position)
                {
                    Stop(res.SaveChanges);
                    return true;
                }
                else
                {
                    Restart();
                    return false;
                }
            }, _algorithmTaskCancellationTokenSource.Token);
            return _algorithmTask;
        }
    }
}
