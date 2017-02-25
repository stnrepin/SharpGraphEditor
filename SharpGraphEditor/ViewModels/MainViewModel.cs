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
        private IGraphElement _selectedElement;
        private IEdge _newEdge;

        private string _title;
        private bool _isAddVertexModeOn;
        private bool _isRemoveElementModeOn;
        private bool _isCursorModeOn;
        private double _currentZoom;
        private bool _isCanvasUnlock;
        private bool _isOutputHide;

        public double MinElementX { get; set; }
        public double MinElementY { get; set; }
        public double MaxElementX { get; set; }
        public double MaxElementY { get; set; }

        public IWindowManager WindowManager { get; }
        public IDialogsPresenter DialogPresenter { get; }

        public ITerminal Terminal { get; set; }

        public double MaxZoom { get; set; } = 2; // Zoom of GraphControl will be 0 if this initializing will be in constructor.
        public List<IAlgorithm> Algorithms { get; set; }

        // Constructors
        //
        public MainViewModel(IWindowManager windowManager, IDialogsPresenter dialogsPresenter)
        {
            Document = new GraphDocument();
            Algorithms = AlgorithmManager.Instance.Algorithms;

            WindowManager = windowManager;
            DialogPresenter = dialogsPresenter;

            CurrentZoom = 1;
            MinElementX = 30;
            MinElementY = 30;

            Init();
        }

        private void Init()
        {
            Title = ProjectName;
            IsUnlock = true;
            IsCursorModeOn = true;
            SelectedElement = null;
            NewEdge = null;
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
        }

        public void LoadGraphFromFile()
        {
            if (CheckGraphForClearing())
            {
                try
                {
                    var dialog = new FileDialogViewModel(DialogPresenter, new Models.FileDialog.OpenDialogType());
                    WindowManager.ShowDialog(dialog);
                    var fileName = dialog.FilePath;

                    if (String.IsNullOrEmpty(fileName)) return;

                    Document.LoadFrom(fileName, dialog.FileType);
                    Title = ProjectName + $" - {fileName}";

                    if (Document.Vertices.All(x => !x.HasPosition))
                    {
                        var alg = AlgorithmManager.Instance.FindAlgorithmByName("Ellipse layouter");
                        if (alg == null)
                        {
                            throw new ArgumentNullException("Cant find Ellipse layouter algorithm");
                        }
                        RunAlgorithm(alg);
                    }
                }
                catch (Exception e)
                {
                    ShowError(e);
                }
            }
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

        public void Save()
        {
            if (String.IsNullOrEmpty(Document.SourceFile))
            {
                SaveAs();
                return;
            }

            try
            {
                Document.SaveTo(Document.SourceFile, Document.SourceFileType);
                Title = ProjectName + $" - {Document.SourceFile}";
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
                var dialog = new FileDialogViewModel(DialogPresenter, new Models.FileDialog.SaveDialogType());
                WindowManager.ShowDialog(dialog);
                var fileName = dialog.FilePath;

                if (String.IsNullOrEmpty(fileName)) return;
                Document.SaveTo(fileName, dialog.FileType);
                Title = ProjectName + $" - {fileName}";
            }
            catch (Exception e)
            {
                ShowError(e);
            }
        }

        public void CanvasClick(double mousePositionX, double mousePositionY, IGraphElement element)
        {
            if (IsCursorModeOn)
            {
                SelectedElement = (element != SelectedElement ? element : null);
                return;
            }
            else if (IsAddElementModeOn)
            {
                if (NewEdge == null)
                {
                    if (element is Vertex)
                    {
                        var sourceVertex = element as IVertex;
                        var newVertex = Document.AddVertex(0, 0);
                        newVertex.IsAdding = true;
                        SelectedElement = sourceVertex;
                        NewEdge = Document.AddEdge(sourceVertex, newVertex);
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
                        RemoveElement(NewEdge);
                        var targetVertex = Document.AddVertex(mousePositionX, mousePositionY);
                        Document.AddEdge(sourceVertex, targetVertex);
                    }
                    RemoveElement(NewEdge);
                }
            }
            else if (IsRemoveElementModeOn)
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

            if (element == NewEdge)
            {
                Document.Remove(NewEdge?.Target);
                Document.Remove(NewEdge);
                NewEdge = null;
            }
            else
            {
                Document.Remove(element);
            }

            if (NewEdge != null)
                RemoveElement(NewEdge);

        }

        public void ChangeEdgeDirection(Edge edge)
        {
            edge.IsDirected = !edge.IsDirected;
        }

        public void ChangeEdgesDirection(bool isDirected)
        {
            Document.IsDirected = isDirected;
        }

        public void RunAlgorithm(IAlgorithm algorithm)
        {
            var param = new AlgorithmParameter()
            {
                Output = Terminal,
                MaxElementX = MaxElementX,
                MaxElementY = MaxElementY,
                MinElementX = MinElementX,
                MinElementY = MinElementY
            };

            var a = new Helpers.AsyncOperation(() =>
            {
                IsUnlock = false;
                Terminal?.WriteLine($"{algorithm.Name} starting...");
                algorithm?.Run(Document, param);
            },
            () =>
            {
                Terminal?.WriteLine("Algorithm finished successfully.\n");
                IsUnlock = true;
            },
            (e) =>
            {
                Terminal?.WriteLine("During algorithm working an error occured:");
                Terminal?.WriteLine($"  {e.Message}\n");
                IsUnlock = true;
            });
            a.ExecuteAsync();
        }

        public void ChangeOutputVisibility(bool value)
        {
            IsOutputHide = value;
        }

        public void ClearTerminalText()
        {
            Terminal?.Clear();
        }

        public void ChangeZoomByPercents(double percents)
        {
            CurrentZoom += percents / 100;
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
                if (_newEdge != null)
                    _newEdge.IsAdding = true;
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

        public double CurrentZoom
        {
            get { return _currentZoom; }
            set
            {
                if (value >= (1 / MaxZoom) && value <= MaxZoom)
                {
                    _currentZoom = Math.Round(value, 2);
                    NotifyOfPropertyChange(() => CurrentZoom);
                    NotifyOfPropertyChange(() => CurrentZoomInPercents);
                }
            }
        }

        public int CurrentZoomInPercents
        {
            get { return (int)(_currentZoom * 100); }
        }

        public bool IsCursorModeOn
        {
            get { return _isCursorModeOn; }
            set
            {
                _isCursorModeOn = value;
                OnModeChanged();
                NotifyOfPropertyChange(() => IsCursorModeOn);
            }
        }

        public bool IsAddElementModeOn
        {
            get { return _isAddVertexModeOn; }
            set
            {
                _isAddVertexModeOn = value;
                OnModeChanged();
                NotifyOfPropertyChange(() => IsAddElementModeOn);
            }
        }

        public bool IsRemoveElementModeOn
        {
            get { return _isRemoveElementModeOn; }
            set
            {
                _isRemoveElementModeOn = value;
                Document.Remove(SelectedElement);
                OnModeChanged();
                NotifyOfPropertyChange(() => IsRemoveElementModeOn);
            }
        }

        // Methods
        //
        private bool CheckGraphForClearing()
        {
            if (Document.IsModified)
            {
                var res = DialogPresenter.ShowMessaeBoxYesNoCancel("Graph has been modified. Save changes?", ProjectName);
                switch (res)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        if (Document.IsModified)
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

        private void OnModeChanged()
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
    }
}
