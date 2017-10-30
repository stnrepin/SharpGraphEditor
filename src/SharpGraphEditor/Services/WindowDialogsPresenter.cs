using System;
using System.Windows;

using Microsoft.Win32;

namespace SharpGraphEditor.Services
{
    public class WindowDialogsPresenter : IDialogsPresenter
    {
        public MessageBoxResult ShowMessaeBoxYesNoCancel(string message, string caption)
        {
            var mainWindow = Application.Current.MainWindow;

            var res = MessageBox.Show(mainWindow, message, caption, MessageBoxButton.YesNoCancel);
            switch (res)
            {
                case System.Windows.MessageBoxResult.Yes:
                    return MessageBoxResult.Yes;
                case System.Windows.MessageBoxResult.No:
                    return MessageBoxResult.No;
                case System.Windows.MessageBoxResult.Cancel:
                    return MessageBoxResult.Cancel;
                default:
                    throw new ArgumentException("Invalid message box result");
            }

        }

        public void ShowError(string message, string caption, Type exType)
        {
            var errorMessage = $"Sorry, but an error occured: \"{message}\".\nLook more in output.";
            var mainWindow = Application.Current.MainWindow;

            MessageBox.Show(mainWindow, errorMessage, caption, MessageBoxButton.OK);
        }

        public string ShowFileOpenDialog(string filter = "")
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                CheckFileExists = true,
                CheckPathExists = true,
                ValidateNames = true,
                DereferenceLinks = true,
                AddExtension = true,
                Filter = filter
            };

            var dialogRes = fileDialog.ShowDialog();
            if (dialogRes.HasValue && dialogRes.Value)
            {
                return fileDialog.FileName;
            }
            return String.Empty;
        }

        public string ShowFileSaveDialog(string filter = "")
        {
            var fileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                CheckPathExists = true,
                ValidateNames = true,
                DereferenceLinks = true,
                OverwritePrompt = true,
                CreatePrompt = true,
                AddExtension = true,
                Filter = filter
            };

            var dialogRes = fileDialog.ShowDialog();
            if (dialogRes.HasValue && dialogRes.Value)
            {
                return fileDialog.FileName;
            }
            return String.Empty;
        }
    }
}
