using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading;

using SharpGraphEditor.Models.Operations;

using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.Algorithms
{
    public class TestAlgorithm : IAlgorithm
    {
        private List<Operation> _lastOperations;
        private GraphDocument _doc;
        private Vertex _vertex;

        public string Name { get; set; } = "Test Visualizer";
        public string Description { get; set; } =
            "Test Visualizer. Add a lot af vertecies.";

        public int CurrentUndoRedoPosition { get; set; } = 0;

        public TestAlgorithm()
        {
            _lastOperations = new List<Operation>();
        }

        public void Run(GraphDocument doc, IDialogsPresenter dialogPresenter)
        {
            _doc = doc;
            _doc.Clear();
            Redo();
        }

        public void Undo()
        {
            if (CurrentUndoRedoPosition == 0) return;

            _lastOperations[--CurrentUndoRedoPosition].Undo();
        }

        public void Redo()
        {
            if (CurrentUndoRedoPosition == _lastOperations.Count)
            {

                if (_vertex == null || _doc.ObservableVertices.Count == 0)
                {
                    var o = new AddVertexOperation(_doc, 200, 200);
                    o.Redo();
                    _vertex = o.Vertex;
                    _lastOperations.Add(o);
                }
                else
                {
                    Action redoAction = () => _vertex.X += 10;
                    Action undoAction = () => _vertex.X -= 10;
                    redoAction();
                    _lastOperations.Add(new Operation(redoAction, undoAction));
                }
            }
            else
            {
                _lastOperations[CurrentUndoRedoPosition].Redo();
            }
            CurrentUndoRedoPosition++;
        }
    }
}
