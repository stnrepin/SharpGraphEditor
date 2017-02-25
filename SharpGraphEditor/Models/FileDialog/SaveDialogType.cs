using System;
using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.FileDialog
{
    public class SaveDialogType : BaseDialogType
    {
        public override string TypeName => "Save";

        public override string Summarize(IDialogsPresenter dialogPresenter, GraphSourceFileType fileType)
        {
            var filter = GetFilterForSourceFileType(fileType);
            return dialogPresenter.ShowFileSaveDialog(filter);
        }
    }
}