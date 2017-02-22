using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.FileDialog
{
    public class OpenDialogType : BaseDialogType
    {
        public override string TypeName => "Open";

        public override string Summarize(IDialogsPresenter dialogPresenter, GraphSourceFileType fileType)
        {
            var filter = GetFilterForSourceFileType(fileType);
            return dialogPresenter.ShowFileOpenDialog(filter);
        }
    }
}
