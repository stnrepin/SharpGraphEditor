using System;

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

        void ShowError(string message, string caption);

        string ShowFileOpenDialog(string filter);

        string ShowFileSaveDialog(string filter);
    }
}
