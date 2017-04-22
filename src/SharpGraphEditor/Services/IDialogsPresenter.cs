using System;
using System.Threading.Tasks;

namespace SharpGraphEditor.Services
{
    public enum MessageBoxResult
    {
        Yes,
        No,
        Cancel
    }

    public interface IDialogsPresenter
    {
        Task<MessageBoxResult> ShowMessaeBoxYesNoCancelAsync(string message, string caption);

        Task ShowErrorAsync(string message, string caption,  Type exType);

        string ShowFileOpenDialog(string filter);

        string ShowFileSaveDialog(string filter);
    }
}
