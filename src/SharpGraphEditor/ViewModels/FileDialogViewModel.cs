using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;
using SharpGraphEditor.Models;
using SharpGraphEditor.Services;

namespace SharpGraphEditor.ViewModels
{
    public enum FileDialogMode
    {
        Open,
        Save
    }

    public class FileDialogViewModel : PropertyChangedBase
    {
        public FileDialogMode Mode { get; private set; }
        public GraphSourceType SourceType { get; private set; }
        public bool IsOpenMode { get; private set; }

        public FileDialogViewModel(FileDialogMode mode)
        {
            Mode = mode;
            SourceType = GraphSourceType.Gxml;

            IsOpenMode = mode == FileDialogMode.Open;
        }

        public void Ok(IClose closeableWindow)
        {
            closeableWindow?.TryClose(true);
        }
        
        public void Cancel(IClose closableWindow)
        {
            closableWindow?.TryClose(false);
        }

        public void ChangeFormat(string name)
        {
            if (Enum.TryParse(name, out GraphSourceType res))
            {
                SourceType = res;
                return;
            }
            throw new ArgumentException("Invalid source file type");
        }
    }
}
