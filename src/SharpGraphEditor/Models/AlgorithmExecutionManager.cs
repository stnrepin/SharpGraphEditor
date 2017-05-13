﻿using System;
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
        private GraphDocument _doc;
        private CancellationTokenSource _algorithmTaskCancellationTokenSource;
        private bool _isAlgorithmExecuting;
        private int _undoRedoStartPosition;

        public int StepInterval { get; set; }

        public AlgorithmExecutionManager(GraphDocument doc)
        {
            _doc = doc;
            _undoRedoStartPosition = _doc.UndoRedoManager.Position;
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
            while (_doc.UndoRedoManager.Position > _undoRedoStartPosition)
            {

                _doc.UndoRedoManager.Undo();
            }
            IsAlgorithmExecuting = false;
        }

        public void StepNext()
        {
            IsAlgorithmExecuting = true;
            _doc.UndoRedoManager.Redo();
            IsAlgorithmExecuting = false;
        }

        public void StepBack()
        {
            IsAlgorithmExecuting = true;
            
            if (_doc.UndoRedoManager.Position > _undoRedoStartPosition)
            {
                _doc.UndoRedoManager.Undo();
            }
            IsAlgorithmExecuting = false;
        }

        public void Stop()
        {
            Restart();
            _doc.UndoRedoManager.CutOff();
            _undoRedoStartPosition = -1;
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
                    while (_doc.UndoRedoManager.CanRedo)
                    {
                        if (_algorithmTaskCancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        _doc.UndoRedoManager.Redo();
                        Thread.Sleep(StepInterval);
                    }
                    IsAlgorithmExecuting = false;
                }, _algorithmTaskCancellationTokenSource.Token);
            }
        }

        public Task<bool> Run(IAlgorithm algorithm, AlgorithmParameter parameter)
        {
            return Task.Factory.StartNew(() =>
            {
                IsAlgorithmExecuting = true;
                algorithm?.Run(_doc, parameter);

                if (_undoRedoStartPosition == _doc.UndoRedoManager.Position)
                {
                    Stop();
                    return true;
                }
                else
                {
                    Restart();
                    return false;
                }
            }, _algorithmTaskCancellationTokenSource.Token);
        }
    }
}
