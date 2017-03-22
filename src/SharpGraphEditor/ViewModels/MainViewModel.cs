using System;
using System.Collections.Generic;
using System.Linq;

using Caliburn.Micro;

using SharpGraphEditor.Models;
using SharpGraphEditor.Services;
using SharpGraphEditor.Controls;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Algorithms;

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
        private bool _isCanvasUnlock;
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
            Algorithms = AlgorithmManager.Instance.Algorithms;

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
            IsUnlock = true;
            CurrentCursorMode = CursorMode.Default;
            SelectedElement = null;
            NewEdge = null;
            IsModified = false;
        }

        // Actions
        //
        public void ShowAbout()
        {
            WindowManager?.ShowDialog(new AboutViewModel());
        }

        public void Exit()
        {
            if (CheckGraphForClearing())
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

        public void ClearGraph()
        {
            if (CheckGraphForClearing())
            {
                Document.Clear();
                Terminal?.Clear();
                ViewLoaded();
                Init();
            }
        }

        public void Redo()
        {
            Document.Redo();
        }

        public void Undo()
        {
            Document.Undo();
        }

        public void ChangeZoomByPercents(double percents)
        {
            _zoomManager.ChangeZoomByPercents(percents);
            NotifyOfPropertyChange(() => CurrentZoom);
            NotifyOfPropertyChange(() => CurrentZoomInPercents);
        }

        public void LoadGraphFromFile()
        {
            if (CheckGraphForClearing())
            {
                try
                {
                    var dialog = new FileDialogViewModel();
                    var res = WindowManager.ShowDialog(dialog);
                    if (res.HasValue && res.Value)
                    {
                        var filter = GetFilterForSourceFileType(dialog.SourceType);
                        var fileName = DialogPresenter.ShowFileOpenDialog(filter);

                        if (String.IsNullOrEmpty(fileName))
                            return;

                        _repository.LoadFromFile(Document, fileName, dialog.SourceType);
                        Title = ProjectName + $" - {fileName}";

                        EllipseVerticesPositionIfNeed();
                        IsModified = false;
                        Document.ClearUndoRedoCache();
                    }
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
        }

        public void LoadGraphFromText()
        {
            if (CheckGraphForClearing())
            {
                try
                {
                    var dialog = new FileDialogViewModel();
                    var res = WindowManager.ShowDialog(dialog);
                    if (res.HasValue && res.Value)
                    {
                        var textViewer = new TextViewerViewModel(String.Empty, false, true, false);
                        var textViewerResult = WindowManager.ShowDialog(textViewer);
                        if (textViewerResult.HasValue && textViewerResult.Value)
                        {
                            _repository.LoadFromText(Document, textViewer.Text, dialog.SourceType);
                            EllipseVerticesPositionIfNeed();
                            IsModified = true;
                            Title = ProjectName;
                        }
                        Document.ClearUndoRedoCache();
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
                var dialog = new FileDialogViewModel();
                var res = WindowManager.ShowDialog(dialog);
                if (res.HasValue && res.Value)
                {
                    var filter = GetFilterForSourceFileType(dialog.SourceType);
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
                var dialog = new FileDialogViewModel();
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
            var mode =_cursorModeManager.Current;

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

        public void RunAlgorithm(IAlgorithm algolithm)
        {
            RunAlgorithm(algolithm, true);
        }

        public void RunAlgorithm(IAlgorithm algorithm, bool checkGraphForClearing)
        {
            if (!checkGraphForClearing || CheckGraphForClearing())
            {
                var a = new Helpers.AsyncOperation(() =>
                {
                    IsUnlock = false;
                    Terminal?.WriteLine($"{algorithm.Name} starting...");
                    algorithm?.Run(Document, AlgorithmParameter);
                },
                () =>
                {
                    Terminal?.WriteLine("Algorithm finished successfully.\n");
                    IsUnlock = true;
                    EllipseVerticesPositionIfNeed();
                },
                (e) =>
                {
                    Terminal?.WriteLine("During algorithm working an error occured:");
                    Terminal?.WriteLine($"  {e.Message}\n");
                    IsUnlock = true;
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

        public bool IsUnlock
        {
            get { return _isCanvasUnlock; }
            set
            {
                _isCanvasUnlock = value;
                NotifyOfPropertyChange(() => IsUnlock);
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
        private bool CheckGraphForClearing()
        {
            if (IsModified)
            {
                var res = DialogPresenter.ShowMessaeBoxYesNoCancel("Graph has been modified. Save changes?", ProjectName);
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

        private void EllipseVerticesPositionIfNeed()
        {
            if (Document.Vertices.All(x => !x.HasPosition))
            {
                var alg = AlgorithmManager.Instance.FindAlgorithmByName("Ellipse layouter");
                if (alg == null)
                {
                    throw new ArgumentNullException("Cant find Ellipse layouter algorithm");
                }
                RunAlgorithm(alg, false);
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

            DialogPresenter.ShowError(ex.Message, ProjectName, ex.GetType());
        }

        private string GetFilterForSourceFileType(GraphSourceType fileType)
        {
            var filter = String.Empty;
            switch (fileType)
            {
                case GraphSourceType.Gxml:
                    filter = "GXML files (*.gxml) | *.gxml";
                    break;
                case GraphSourceType.AdjList:
                case GraphSourceType.AdjMatrix:
                case GraphSourceType.EdgesList:
                case GraphSourceType.IncidenceMatrix:
                    filter = "TXT files (*.txt) | *.txt";
                    break;
                default:
                    throw new ArgumentException("bad file type");
            }
            return filter;
        }
    }
}
