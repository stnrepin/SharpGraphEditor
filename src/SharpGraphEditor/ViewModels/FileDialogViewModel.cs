using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Caliburn.Micro;
using SharpGraphEditor.Models;
using SharpGraphEditor.Services;

namespace SharpGraphEditor.ViewModels
{
    public class FileDialogViewModel : PropertyChangedBase
    {
        public GraphSourceType SourceType { get; private set; }

        public FileDialogViewModel()
        {
            SourceType = GraphSourceType.Gxml;
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
