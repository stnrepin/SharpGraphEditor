using System;
using System.Collections.Generic;
using System.Linq;

using Caliburn.Micro;

using SharpGraphEditor.Models;
using SharpGraphEditor.Services;
using SharpGraphEditor.Controls;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Algorithms;
using System.Threading;

namespace SharpGraphEditor.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        // Constants
        //
        private readonly string ProjectName = "#GraphEditor";

        // Fields
        //
        private GraphDocument _document;
        private GraphRepository _repository;
        private IGraphElement _selectedElement;
        private CursorModeManager _cursorModeManager;
        private ZoomManager _zoomManager;
        private IEdge _newEdge;

        private string _title;
        private bool _isAlgorithmRun;
        private bool _isAlgorithmExecuting;
        private bool _isOutputHide;
        private int _undoRedoCachePositionOfAlgorithm;
        private CancellationTokenSource _algorithmTaskCancellationTokenSource;

        public bool IsModified { get; private set; }

        public AlgorithmParameter AlgorithmParameter { get; set; }

        public IWindowManager WindowManager { get; }
        public IDialogsPresenter DialogPresenter { get; }

        public ITerminal Terminal { get; set; }

        public List<IAlgorithm> Algorithms { get; set; }

        // Constructors
        //
        public MainViewModel(IWindowManager windowManager, IDialogsPresenter dialogsPresenter)
        {
            Document = new GraphDocument();
            AlgorithmParameter = new AlgorithmParameter();
            Algorithms = AlgorithmProvider.Instance.Algorithms;

            _repository = new GraphRepository();
            _cursorModeManager = new CursorModeManager();
            _zoomManager = new ZoomManager();

            _algorithmTaskCancellationTokenSource = new CancellationTokenSource();

            WindowManager = windowManager;
            DialogPresenter = dialogsPresenter;

            AlgorithmParameter.MinElementX = 30;
            AlgorithmParameter.MinElementY = 30;

            Document.GraphDocumentChanged += (_, __) => { IsModified = true; };
            Init();
        }

        private void Init()
        {
            Title = ProjectName;
            IsAlgorithmRun = false;
            IsAlgorithmExecuting = false;
            CurrentCursorMode = CursorMode.Default;
            SelectedElement = null;
            NewEdge = null;
            IsModified = false;

            _undoRedoCachePositionOfAlgorithm = -1;
        }

        // Actions
        //
        public void ShowAbout()
        {
            WindowManager?.ShowDialog(new AboutViewModel());
        }

        public void ShowVertexProperties(IVertex vertex)
        {
            if (vertex == null)
            {
                if (SelectedElement is IVertex v)
                {
                    vertex = v;
                }
                else
                {
                    return;
                }
            }
            WindowManager?.ShowDialog(new VertexPropertiesViewModel(vertex));
        }

        public async System.Threading.Tasks.Task ExitAsync()
        {
            if (await CheckGraphForClearingAsync())
            {
                Environment.Exit(0);
            }
        }

        public void ViewLoaded()
        {
            Terminal?.WriteLine(ProjectName);
            Terminal?.WriteLine("(c) Stepan Repin, 2017");
            Terminal?.WriteLine();

            AlgorithmParameter.Output = Terminal;
        }

        public async System.Threading.Tasks.Task ClearGraphAsync()
        {
            if (await CheckGraphForClearingAsync())
            {
                Document.Clear();
                Terminal?.Clear();
                ViewLoaded();
                Init();
            }
        }

        public void Redo()
        {
            Document.UndoRedoManager.Redo();
        }

        public void Undo()
        {
            Document.UndoRedoManager.Undo();
        }

        public void ChangeZoomByPercents(double percents)
        {
            _zoomManager.ChangeZoomByPercents(percents);
            NotifyOfPropertyChange(() => CurrentZoom);
            NotifyOfPropertyChange(() => CurrentZoomInPercents);
        }

        public async System.Threading.Tasks.Task LoadGraphFromFileAsync()
        {
            if (await CheckGraphForClearingAsync())
            {
                try
                {
                    var dialog = new FileDialogViewModel(FileDialogMode.Open);
                    var res = WindowManager.ShowDialog(dialog);
                    if (res.HasValue && res.Value)
                    {
                        var filter = _repository.GetFilterForSourceFileType(dialog.SourceType);
                        var fileName = DialogPresenter.ShowFileOpenDialog(filter);

                        if (String.IsNullOrEmpty(fileName))
                            return;

                        _repository.LoadFromFile(Document, fileName, dialog.SourceType);

                        await EllipseVerticesPositionIfNeedAsync();
                        Document.UndoRedoManager.Clear();
                        Init();
                        Title = ProjectName + $" - {fileName}";
                    }
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        }

        public async System.Threading.Tasks.Task LoadGraphFromTextAsync()
        {
            if (await CheckGraphForClearingAsync())
            {
                try
                {
                    var dialog = new FileDialogViewModel(FileDialogMode.Open);
                    var res = WindowManager.ShowDialog(dialog);
                    if (res.HasValue && res.Value)
                    {
                        var textViewer = new TextViewerViewModel(String.Empty, false, true, false);
                        var textViewerResult = WindowManager.ShowDialog(textViewer);
                        if (textViewerResult.HasValue && textViewerResult.Value)
                        {
                            _repository.LoadFromText(Document, textViewer.Text, dialog.SourceType);
                            await EllipseVerticesPositionIfNeedAsync();
                            IsModified = true;
                            Title = ProjectName;
                        }
                        Document.UndoRedoManager.Clear();
                        Init();
                    }
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        }

        public void Save()
        {
            if (String.IsNullOrEmpty(_repository.SourceFile))
            {
                SaveAs();
                return;
            }

            try
            {
                _repository.SaveToFile(Document, _repository.SourceFile, _repository.SourceType);
                Title = ProjectName + $" - {_repository.SourceFile}";
                IsModified = false;
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public void SaveAs()
        {
            try
            {
                var dialog = new FileDialogViewModel(FileDialogMode.Save);
                var res = WindowManager.ShowDialog(dialog);
                if (res.HasValue && res.Value)
                {
                    var filter = _repository.GetFilterForSourceFileType(dialog.SourceType);
                    var fileName = DialogPresenter.ShowFileSaveDialog(filter);

                    if (String.IsNullOrEmpty(fileName))
                        return;

                    _repository.SaveToFile(Document, fileName, dialog.SourceType);
                    Title = ProjectName + $" - {fileName}";
                    IsModified = false;
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public void SaveAsText()
        {
            try
            {
                var dialog = new FileDialogViewModel(FileDialogMode.Save);
                var res = WindowManager.ShowDialog(dialog);
                if (res.HasValue && res.Value)
                {
                    var text = _repository.PresentAsText(Document, dialog.SourceType);
                    var textViewer = new TextViewerViewModel(text, true, false, true);
                    WindowManager.ShowDialog(textViewer);
                }
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public void CanvasClick(double mousePositionX, double mousePositionY, IGraphElement element)
        {
            var mode = _cursorModeManager.Current;

            if (mode == CursorMode.Default)
            {
                SelectedElement = (element != SelectedElement ? element : null);
                return;
            }
            else if (mode == CursorMode.Add)
            {
                if (NewEdge == null)
                {
                    if (element is Vertex)
                    {
                        var sourceVertex = element as IVertex;
                        SelectedElement = sourceVertex;
                        var targetVertex = new Vertex(mousePositionX, mousePositionY) { IsAdding = true };
                        NewEdge = new Edge(sourceVertex, targetVertex, false) { IsAdding = true };
                        return;
                    }
                    else
                    {
                        Document.AddVertex(mousePositionX, mousePositionY);
                    }
                }
                else
                {
                    if (element is Vertex)
                    {
                        Document.AddEdge(NewEdge.Source, element as IVertex);
                    }
                    else
                    {
                        var sourceVertex = NewEdge.Source;
                        NewEdge = null;
                        var targetVertex = Document.AddVertex(mousePositionX, mousePositionY);
                        Document.AddEdge(sourceVertex, targetVertex);
                    }
                    NewEdge = null;
                }
            }
            else if (mode == CursorMode.Remove)
            {
                RemoveElement(element);
            }
            SelectedElement = null;
        }

        public void RemoveElement(IGraphElement element)
        {
            if (element == null)
            {
                return;
            }

            NewEdge = null;
            SelectedElement = null;
            Document.Remove(element);
        }

        public void ChangeEdgeDirection(Edge edge)
        {
            edge.IsDirected = !edge.IsDirected;
        }

        public void ChangeEdgesDirection(bool isDirected)
        {
            Document.IsDirected = isDirected;
        }

        public void RestartAlgorithm()
        {
            IsAlgorithmExecuting = true;
            while (Document.UndoRedoManager.Position != _undoRedoCachePositionOfAlgorithm)
            {
                Document.UndoRedoManager.Undo();
            }
            IsAlgorithmExecuting = false;
        }

        public void AlgorithmStepNext()
        {
            IsAlgorithmExecuting = true;
            Document.UndoRedoManager.Redo();
            IsAlgorithmExecuting = false;
        }

        public void AlgorithmStepBack()
        {
            if (Document.UndoRedoManager.Position > _undoRedoCachePositionOfAlgorithm)
            {
                IsAlgorithmExecuting = true;
                Document.UndoRedoManager.Undo();
                IsAlgorithmExecuting = false;
            }
        }

        public void StopAlgorithm()
        {
            RestartAlgorithm();
            Document.UndoRedoManager.CutOff();
            _undoRedoCachePositionOfAlgorithm = -1;
            IsAlgorithmRun = false;
            IsAlgorithmExecuting = false;
        }

        public void ContinueOrPauseAlgorithm()
        {
            if (IsAlgorithmExecuting)
            {
                _algorithmTaskCancellationTokenSource.Cancel();
                IsAlgorithmExecuting = false;
            }
            else
            {
                if (!Document.UndoRedoManager.CanRedo)
                {
                    RestartAlgorithm();
                }

                IsAlgorithmExecuting = true;
                _algorithmTaskCancellationTokenSource = new CancellationTokenSource();
                var task = System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    while (Document.UndoRedoManager.CanRedo)
                    {
                        if (_algorithmTaskCancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        Document.UndoRedoManager.Redo();
                        Thread.Sleep(700);
                    }
                    IsAlgorithmExecuting = false;
                }, _algorithmTaskCancellationTokenSource.Token);
            }
        }

        public async System.Threading.Tasks.Task RunAlgorithmAsync(IAlgorithm algolithm)
        {
            await RunAlgorithmAsync(algolithm, true);
        }

        public async System.Threading.Tasks.Task RunAlgorithmAsync(IAlgorithm algorithm, bool checkGraphForClearing)
        {
            if (Document.ObservableVertices.Count == 0)
            {
                return;
            }

            var canClearGraph = await CheckGraphForClearingAsync();
            if (!checkGraphForClearing || canClearGraph)
            {
                var a = new Helpers.AsyncOperation(() =>
                {
                    _undoRedoCachePositionOfAlgorithm = Document.UndoRedoManager.Position;
                    IsAlgorithmRun = true;
                    IsAlgorithmExecuting = true;
                    Terminal?.WriteLine($"{algorithm.Name} starting...");
                    algorithm?.Run(Document, AlgorithmParameter);

                    if (_undoRedoCachePositionOfAlgorithm == Document.UndoRedoManager.Position)
                    {
                        StopAlgorithm();
                    }
                    else
                    {
                        RestartAlgorithm();
                    }
                },
                async () =>
                {
                    Terminal?.WriteLine("Algorithm finished successfully.\n");
                    IsAlgorithmExecuting = false;
                    await EllipseVerticesPositionIfNeedAsync();
                },
                (e) =>
                {
                    Terminal?.WriteLine("During algorithm working an error occured:");
                    Terminal?.WriteLine($"  {e.Message}\n");
                    StopAlgorithm();
                });
                a.ExecuteAsync();
            }
        }

        public void ChangeOutputVisibility(bool value)
        {
            IsOutputHide = value;
        }

        public void ClearTerminalText()
        {
            Terminal?.Clear();
        }

        // Properties
        //
        public GraphDocument Document
        {
            get { return _document; }
            set
            {
                _document = value;
                NotifyOfPropertyChange(() => Document);
            }
        }

        public IGraphElement SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                if (NewEdge != null)
                    return;

                if (value != _selectedElement)
                {
                    _selectedElement = value;
                    NotifyOfPropertyChange(() => SelectedElement);
                }
            }
        }

        public IEdge NewEdge
        {
            get { return _newEdge; }
            set
            {
                _newEdge = value;
                NotifyOfPropertyChange(() => NewEdge);
            }
        }

        public bool IsAlgorithmRun
        {
            get { return _isAlgorithmRun; }
            set
            {
                _isAlgorithmRun = value;
                NotifyOfPropertyChange(() => IsAlgorithmRun);
            }
        }

        public bool IsAlgorithmExecuting
        {
            get { return _isAlgorithmExecuting; }
            set
            {
                _isAlgorithmExecuting = value;
                NotifyOfPropertyChange(() => IsAlgorithmExecuting);
            }
        }

        public bool IsOutputHide
        {
            get { return _isOutputHide; }
            set
            {
                _isOutputHide = value;
                NotifyOfPropertyChange(() => IsOutputHide);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public CursorMode CurrentCursorMode
        {
            get { return _cursorModeManager.Current; }
            set
            {
                _cursorModeManager.Change(value);
                OnCursorModeChanged();
                NotifyOfPropertyChange(() => CurrentCursorMode);
            }
        }

        public int CurrentZoomInPercents
        {
            get { return _zoomManager.CurrentZoomInPercents; }
        }

        public double CurrentZoom
        {
            get { return _zoomManager.CurrentZoom; }
        }

        // Methods
        //
        private async System.Threading.Tasks.Task<bool> CheckGraphForClearingAsync()
        {
            if (IsModified)
            {
                var res = await DialogPresenter.ShowMessaeBoxYesNoCancelAsync("Graph has been modified. Save changes?", ProjectName);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        if (IsModified)
                            return false;
                        return true;
                    case MessageBoxResult.No:
                        return true;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            return true;
        }

        private async System.Threading.Tasks.Task EllipseVerticesPositionIfNeedAsync()
        {
            if (Document.Vertices.All(x => !x.HasPosition))
            {
                var alg = AlgorithmProvider.Instance.FindAlgorithmByName("Ellipse layouter");
                if (alg == null)
                {
                    throw new ArgumentNullException("Cant find Ellipse layouter algorithm");
                }
                await RunAlgorithmAsync(alg, false);
            }
        }

        private void OnCursorModeChanged()
        {
            SelectedElement = null;
        }

        private void ShowError(Exception ex)
        {
            if (Terminal != null)
            {
                var exCopy = ex;
                Terminal.WriteLine("An error occured:");
                do
                {
                    Terminal.WriteLine($"  {exCopy.GetType().Name}: \"{exCopy.Message}\"");
                    exCopy = exCopy.InnerException;
                }
                while (exCopy != null);
            }

            DialogPresenter.ShowErrorAsync(ex.Message, ProjectName, ex.GetType());
        }
    }
}
