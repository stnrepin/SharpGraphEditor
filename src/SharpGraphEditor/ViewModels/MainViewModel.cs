using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Caliburn.Micro;

using SharpGraphEditor.Graph.Core.Elements;
using SharpGraphEditor.Graph.Core.Algorithms;
using SharpGraphEditor.Graph.Core;

using SharpGraphEditor.Models;
using SharpGraphEditor.Services;
using SharpGraphEditor.Controls;
using SharpGraphEditor.Extentions;

namespace SharpGraphEditor.ViewModels
{
    public class MainViewModel : PropertyChangedBase, IAlgorithmHost
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
        private NewEdge _newEdge;

        private AutoResetEvent _eventWaiter;

        private string _title;
        private int _lastSavingUndoRedoOperationsCount;
        private bool _isAlgorithmRun;
        private bool _isAlgorithmControlPanelEnabled;
        private bool _isOutputHide;
        private string _commentText;
        private bool _isCommentVisible;
        private bool _isTableVisible;
        private bool _isNewEdgeExists;

        public bool IsModified => _lastSavingUndoRedoOperationsCount != UndoRedoManager.Instance.Position;

        public ObservableCollection<TableRow> TableRows { get; private set; }

        public IWindowManager WindowManager { get; }
        public IDialogsPresenter DialogPresenter { get; }

        public ITerminal Terminal { get; set; }

        public List<IAlgorithm> Algorithms { get; set; }

        public IAlgorithmOutput Output { get; set; }
        public double MinElementX { get; set; }
        public double MinElementY { get; set; }
        public double MaxElementX { get; set; }
        public double MaxElementY { get; set; }

        // Constructors
        //
        public MainViewModel(IWindowManager windowManager, IDialogsPresenter dialogsPresenter)
        {
            Document = new GraphDocument();
            Algorithms = AlgorithmProvider.Instance.Algorithms;

            _repository = new GraphRepository();
            _cursorModeManager = new CursorModeManager();
            _zoomManager = new ZoomManager();

            _eventWaiter = new AutoResetEvent(false);

            TableRows = new ObservableCollection<TableRow>();

            WindowManager = windowManager;
            DialogPresenter = dialogsPresenter;

            MinElementX = 30;
            MinElementY = 30;

            IsNewEdgeEnabled = true;
            NewEdge = new NewEdge(new Vertex(0, 0), 0, 0);

            Init();
        }

        private void Init()
        {
            Title = ProjectName;
            CurrentCursorMode = CursorMode.Default;
            SelectedElement = null;
            IsNewEdgeEnabled = false;

            UnmodifyDocument();

            if (IsAlgorithmRun)
            {
                StopAlgorithm();
            }
            AlgorithmExecutor = null;

            PropertyChanged -= SelectedElementPropertyChanged;

            ClearComment();
            HideComment();

            ClearTable();
            HideTable();
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

        public async System.Threading.Tasks.Task GenerateGraphAsync()
        {
            var generatorDialog = new GraphGeneratorViewModel(Document.ObservableVertices.Count);
            var res = WindowManager.ShowDialog(generatorDialog);
            if (res.HasValue && res.Value)
            {
                var generator = new GraphGenerator();
                var edgesList = generator.GenerateEdgesList(generatorDialog.Dense, generatorDialog.VerticesCount);
                if (await CheckGraphForClearingAsync())
                {
                    Init();
                    _repository.LoadFromText(Document, edgesList, GraphSourceType.EdgesList);

                    // if some vertices have not edges.
                    for (int i = 1; i <= generatorDialog.VerticesCount; i++)
                    {
                        Document.AddVertex(i);
                    }

                    await EllipseVerticesPositionIfNeedAsync();
                }
            }
        }

        public void ShowGraphProperties()
        {
            var graphPropertiesView = new GraphPropertiesViewModel(Document);
            WindowManager?.ShowDialog(graphPropertiesView);
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

            // It's important line. Without it PropertyChangedCallback in MatrixSourceProperty in ListViewWithGridViewBehavior WILL NOT be fired.
            // Do not remove same line in constructor, too.
            // Magic O_o
            TableRows = new ObservableCollection<TableRow>();
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
            UndoRedoManager.Instance.Redo();
        }

        public void Undo()
        {
            if (IsAlgorithmRun)
            {
                return;
            }
            UndoRedoManager.Instance.Undo();
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
                        UndoRedoManager.Instance.Clear();
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
                            UndoRedoManager.Instance.Clear();
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
                UnmodifyDocument();
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
                    UnmodifyDocument();
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
                if (!IsNewEdgeEnabled)
                {
                    if (element is Vertex)
                    {
                        var sourceVertex = element as Vertex;
                        SelectedElement = sourceVertex;
                        NewEdge.Source = sourceVertex;
                        NewEdge.Target.X = mousePositionX;
                        NewEdge.Target.Y = mousePositionY;
                        IsNewEdgeEnabled = true;
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
                        IsNewEdgeEnabled = false;
                        var targetVertex = Document.AddVertex(mousePositionX, mousePositionY);
                        Document.AddEdge(sourceVertex, targetVertex);
                    }
                    IsNewEdgeEnabled = false;
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

            IsNewEdgeEnabled = false;
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
            AlgorithmExecutor.Stop(false);
            IsAlgorithmRun = false;
            IsAlgorithmControlPanelEnabled = false;
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
            Output = Terminal;
            AlgorithmExecutor = new AlgorithmExecutionManager();

            if (!checkGraphForClearing || await CheckGraphForClearingAsync())
            {
                try
                {
                    IsAlgorithmRun = true;
                    IsAlgorithmControlPanelEnabled = true;
                    Terminal?.WriteLine($"{algorithm.Name} starting...");
                    IsAlgorithmRun = !(await AlgorithmExecutor.Run(algorithm, Document, this));
                    Terminal?.WriteLine("Algorithm finished successfully.\n");
                }
                catch (Exception e)
                {
                    Terminal?.WriteLine("During algorithm working an error occured:");
                    Terminal?.WriteLine($"  {e.Message}\n");
                    StopAlgorithm();
                }
                ClearComment();
                HideComment();
                ClearTable();
                HideTable();
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

        public void SetCurrentCursorMode(string mode)
        {
            _cursorModeManager.Change(mode);
            OnCursorModeChanged();
            NotifyOfPropertyChange(() => CurrentCursorMode);
        }

        public IVertex GetSelectedVertex()
        {
            _cursorModeManager.Change(CursorMode.Default);
            NotifyOfPropertyChange(() => CurrentCursorMode);

            CommentText = "Please, select vertex.";
            ShowComment();

            IsAlgorithmControlPanelEnabled = false;
            PropertyChanged += SelectedElementPropertyChanged;

            do
            {
                _eventWaiter.WaitOne();
            }
            while (!(SelectedElement is IVertex));

            PropertyChanged -= SelectedElementPropertyChanged;

            IsAlgorithmControlPanelEnabled = true;
            HideComment();
            return SelectedElement as IVertex;
        }

        public void ShowComment()
        {
            if (!String.IsNullOrEmpty(CommentText))
            {
                IsCommentVisible = true;
            }
        }

        public void ShowComment(string text)
        {
            var op = CreateCommentingOperation(text);
            UndoRedoManager.Instance.AddAndExecute(op);
        }

        public void ShowCommentForLastAction(string text)
        {
            var op = CreateCommentingOperation(text);
            UndoRedoManager.Instance.AppendLastAndExecute(op);
        }

        public void HideComment()
        {
            IsCommentVisible = false;
        }

        public void ClearComment()
        {
            CommentText = String.Empty;
        }

        public void ShowTable()
        {
            IsTableVisible = true;
        }

        public void AddToTable(string row)
        {
            var op = CreateAddingToTableOperation(row);
            UndoRedoManager.Instance.AddAndExecute(op);
        }

        public void AddToTableForLastAction(string row)
        {
            var op = CreateAddingToTableOperation(row);
            UndoRedoManager.Instance.AppendLastAndExecute(op);
        }

        public void AddToTable(string[] row)
        {
            var op = CreateAddingToTableOperation(row);
            UndoRedoManager.Instance.AddAndExecute(op);
        }

        public void AddToTableForLastAction(string[] row)
        {
            var op = CreateAddingToTableOperation(row);
            UndoRedoManager.Instance.AppendLastAndExecute(op);
        }

        public void RemoveRowFromTable(string row)
        {
            var op = CreateRowRemovingFromTableOperation(new[] { row });
            UndoRedoManager.Instance.AddAndExecute(op);
        }

        public void RemoveRowFromTableForLastAction(string row)
        {
            var op = CreateRowRemovingFromTableOperation(new[] { row });
            UndoRedoManager.Instance.AppendLastAndExecute(op);
        }

        public void RemoveRowFromTable(string[] row)
        {
            var op = CreateRowRemovingFromTableOperation(row);
            UndoRedoManager.Instance.AddAndExecute(op);
        }

        public void RemoveRowFromTableForLastAction(string[] row)
        {
            var op = CreateRowRemovingFromTableOperation(row);
            UndoRedoManager.Instance.AppendLastAndExecute(op);
        }

        public void HideTable()
        {
            IsTableVisible = false;
        }

        public void ClearTable()
        {
            TableRows.Clear();
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
                if (IsNewEdgeEnabled)
                {
                    return;
                }

                if (value != _selectedElement)
                {
                    _selectedElement = value;
                    NotifyOfPropertyChange(() => SelectedElement);
                }
            }
        }

        public NewEdge NewEdge
        {
            get { return _newEdge; }
            set
            {
                _newEdge = value;
                if (IsNewEdgeEnabled)
                {
                    NotifyOfPropertyChange(() => NewEdge);
                }
            }
        }

        public bool IsNewEdgeEnabled
        {
            get { return _isNewEdgeExists; }
            set
            {
                if (_isNewEdgeExists != value)
                {
                    _isNewEdgeExists = value;
                    NotifyOfPropertyChange(() => IsNewEdgeEnabled);
                }
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

        public bool IsAlgorithmControlPanelEnabled
        {
            get { return _isAlgorithmControlPanelEnabled; }
            set
            {
                _isAlgorithmControlPanelEnabled = value;
                NotifyOfPropertyChange(() => IsAlgorithmControlPanelEnabled);
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

        public bool IsCommentVisible
        {
            get { return _isCommentVisible; }
            set
            {
                _isCommentVisible = value;
                NotifyOfPropertyChange(() => IsCommentVisible);
            }
        }

        public string CommentText
        {
            get { return _commentText; }
            set
            {
                _commentText = value;
                NotifyOfPropertyChange(() => CommentText);
            }
        }

        public bool IsTableVisible
        {
            get { return _isTableVisible; }
            set
            {
                _isTableVisible = value;
                NotifyOfPropertyChange(() => IsTableVisible);
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

        private void UnmodifyDocument()
        {
            _lastSavingUndoRedoOperationsCount = UndoRedoManager.Instance.Position;
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
            IsNewEdgeEnabled = false;
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


        private void SelectedElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedElement))
            {
                _eventWaiter.Set();
            }
        }

        private IOperation CreateCommentingOperation(string text)
        {
            var tempComment = CommentText;
            var tempIsCommentVisible = IsCommentVisible;
            System.Action redo = () =>
            {
                CommentText = text;
                ShowComment();
            };
            System.Action undo = () =>
            {
                CommentText = tempComment;
                if (tempIsCommentVisible)
                {
                    ShowComment();
                }
                else
                {
                    HideComment();
                }
            };

            return new SimpleOperation(redo, undo);
        }

        private IOperation CreateAddingToTableOperation(params string[] row)
        {
            var tempIsTableVisibile = IsTableVisible;
            var newRow = new TableRow(row);
            System.Action redo = () =>
            {
                Execute.OnUIThread(() =>
                {
                    TableRows.Add(newRow);
                    NotifyOfPropertyChange(() => TableRows);
                });
                ShowTable();
            };
            System.Action undo = () =>
            {
                Execute.OnUIThread(() =>
                {
                    TableRows.RemoveLast(newRow);
                    NotifyOfPropertyChange(() => TableRows);
                });
                if (tempIsTableVisibile)
                {
                    ShowTable();
                }
                else
                {
                    HideTable();
                }
            };

            return new SimpleOperation(redo, undo);
        }

        private IOperation CreateRowRemovingFromTableOperation(string[] row)
        {
            TableRow tempDeletedRow = new TableRow(row);
            System.Action redo = () =>
            {
                Execute.OnUIThread(() =>
                {
                    if (!TableRows.Remove(new TableRow(row)))
                    {
                        tempDeletedRow = null;
                        return;
                    }
                    NotifyOfPropertyChange(() => TableRows);
                });
            };
            System.Action undo = () =>
            {
                Execute.OnUIThread(() =>
                {
                    if (tempDeletedRow != null)
                    {
                        TableRows.Add(tempDeletedRow);
                        NotifyOfPropertyChange(() => TableRows);
                    }
                });
            };

            return new SimpleOperation(redo, undo);
        }
    }
}
