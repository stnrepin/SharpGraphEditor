using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;
using SharpGraphEditor.Models;
using SharpGraphEditor.Models.FileDialog;
using SharpGraphEditor.Services;

namespace SharpGraphEditor.ViewModels
{
    public class FileDialogViewModel : PropertyChangedBase
    {
        private IDialogsPresenter _dialogsPresenter;

        public IDialogType DialogType { get; }

        public GraphSourceFileType FileType { get; private set; }
        public string FilePath { get; private set; }

        public string SummarizeButtonText => DialogType.TypeName;

        public FileDialogViewModel(IDialogsPresenter dialogPresenter, IDialogType type)
        {
            _dialogsPresenter = dialogPresenter;
            DialogType = type;

            FileType = GraphSourceFileType.Gxml;
        }

        public void Summarize(IClose closeableWindow)
        {
            FilePath = DialogType.Summarize(_dialogsPresenter, FileType);
            closeableWindow?.TryClose();
        }
        
        public void Cancel(IClose closableWindow)
        {
            closableWindow?.TryClose();
        }

        public void ChangeFormat(string name)
        {
            if (Enum.TryParse(name, out GraphSourceFileType res))
            {
                FileType = res;
                return;
            }
            throw new ArgumentException("Invalid source file type");
        }
    }
}
