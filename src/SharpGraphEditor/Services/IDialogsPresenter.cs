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
        MessageBoxResult ShowMessaeBoxYesNoCancel(string message, string caption);

        void ShowError(string message, string caption,  Type exType);

        string ShowFileOpenDialog(string filter);

        string ShowFileSaveDialog(string filter);
    }
}
