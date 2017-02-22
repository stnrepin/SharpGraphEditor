using SharpGraphEditor.Services;

namespace SharpGraphEditor.Models.FileDialog
{
    public interface IDialogType
    {
        string TypeName { get; }

        string Summarize(IDialogsPresenter dialogPresenter, GraphSourceFileType fileType);
    }
}
