using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.FileDialog
{
    public abstract class BaseDialogType : IDialogType
    {
        public abstract string TypeName { get; }

        public abstract string Summarize(IDialogsPresenter dialogPresenter, GraphSourceFileType fileType);

        protected string GetFilterForSourceFileType(GraphSourceFileType fileType)
        {
            var filter = String.Empty;
            switch (fileType)
            {
                case GraphSourceFileType.Gxml:
                    filter = "GXML files (*.gxml) | *.gxml";
                    break;
                case GraphSourceFileType.AdjList:
                case GraphSourceFileType.AdjMatrix:
                case GraphSourceFileType.EdgesList:
                case GraphSourceFileType.IncidenceMatrix:
                    filter = "TXT files (*.txt) | *.txt";
                    break;
                default:
                    throw new ArgumentException("bad file type");
            }
            return filter;
        }
    }
}
