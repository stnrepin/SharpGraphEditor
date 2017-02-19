using System;
using System.Text;
using System.Windows;
using Microsoft.Win32;

namespace SharpGraphEditor.Services
{
    public class WindowsDialogsPresenter : IDialogsPresenter
    {
        public MessageBoxResult ShowMessaeBoxYesNoCancel(string message, string caption)
        {
            var res = MessageBox.Show(message, caption, MessageBoxButton.YesNoCancel);
            switch(res)
            {
                case System.Windows.MessageBoxResult.Yes:
                    return MessageBoxResult.Yes;
                case System.Windows.MessageBoxResult.No:
                    return MessageBoxResult.No;
                case System.Windows.MessageBoxResult.Cancel:
                    return MessageBoxResult.Cancel;
                default: throw new ArgumentException("Invalid message box result");
            }
        }


        public void ShowError(string message, string caption)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sorry, but an error occured :(");
            sb.AppendLine();
            sb.AppendLine(message);
            MessageBox.Show(sb.ToString(), caption, MessageBoxButton.OK, MessageBoxImage.Error, 
                System.Windows.MessageBoxResult.OK);
        }

        public void ShowError(string message, string caption, Type exType)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sorry, but an error occured :(");
            sb.AppendLine();
            sb.AppendLine($"Error type: \"{exType.Name}\".");
            sb.AppendLine($"Error message: \"{message}\".");
            sb.AppendLine($"Look more in output.");
            MessageBox.Show(sb.ToString(), caption, MessageBoxButton.OK, MessageBoxImage.Error, 
                System.Windows.MessageBoxResult.OK);
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
