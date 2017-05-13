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
        private ZoomManager _zoomManager;
        private CursorModeManager _cursorModeManager;
        private AlgorithmExecutionManager _algorithmExecutor;
        private IGraphElement _selectedElement;
        private IEdge _newEdge;

        private string _title;
        private bool _isAlgorithmRun;
        private bool _isOutputHide;

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
            CurrentCursorMode = CursorMode.Default;
            SelectedElement = null;
            NewEdge = null;
            IsModified = false;
            AlgorithmExecutor = null;
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

        public async System.Threading.Tasks.Task ShowGraphGeneratorAsync()
        {
            if (await CheckGraphForClearingAsync())
            {
                Init();
                var generator = new GraphGeneratorViewModel(Document.ObservableVertices.Count);
                WindowManager.ShowDialog(generator);
                var edgesList = generator.ResultEdgesList;
                _repository.LoadFromText(Document, edgesList, GraphSourceType.EdgesList);

                for (int i = 1; i <= generator.VerticesCount; i++)
                {
                    Document.AddVertex(i);
                }

                await EllipseVerticesPositionIfNeedAsync();
            }
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
                Terminal?.Clear();
                ViewLoaded();
                Document.Clear();
                Init();
            }
        }

        public void Redo()
        {
            if (IsAlgorithmRun)
            {
                return;
            }
            Document.UndoRedoManager.Redo();
        }

        public void Undo()
        {
            if (IsAlgorithmRun)
            {
                return;
            }
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
                            Document.UndoRedoManager.Clear();
                            Init();
                        }
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
            AlgorithmExecutor.Restart();
        }

        public void AlgorithmStepNext()
        {
            AlgorithmExecutor.StepNext();
        }

        public void AlgorithmStepBack()
        {
            AlgorithmExecutor.StepBack();
        }

        public void StopAlgorithm()
        {
            AlgorithmExecutor.Stop();
            IsAlgorithmRun = false;
        }

        public void ContinueOrPauseAlgorithm()
        {
            AlgorithmExecutor.ContinueOrPause();
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

            SelectedElement = null;
            AlgorithmExecutor = new AlgorithmExecutionManager(Document);

            if (!checkGraphForClearing || await CheckGraphForClearingAsync())
            {
                try
                {
                    IsAlgorithmRun = true;
                    Terminal?.WriteLine($"{algorithm.Name} starting...");
                    IsAlgorithmRun = !(await AlgorithmExecutor.Run(algorithm, AlgorithmParameter));
                    Terminal?.WriteLine("Algorithm finished successfully.\n");
                }
                catch (Exception e)
                {
                    Terminal?.WriteLine("During algorithm working an error occured:");
                    Terminal?.WriteLine($"  {e.Message}\n");
                    StopAlgorithm();
                }

                AlgorithmExecutor.IsAlgorithmExecuting = false;
                await EllipseVerticesPositionIfNeedAsync();
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
                if (_isAlgorithmRun != value)
                {
                    _isAlgorithmRun = value;
                    NotifyOfPropertyChange(() => IsAlgorithmRun);
                }
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

        public AlgorithmExecutionManager AlgorithmExecutor
        {
            get { return _algorithmExecutor; }
            set
            {
                _algorithmExecutor = value;
                NotifyOfPropertyChange(() => AlgorithmExecutor);
            }
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
