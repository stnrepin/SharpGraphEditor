using System;
using System.Text;
using System.Windows;

using Microsoft.Win32;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace SharpGraphEditor.Services
{
    public class WindowDialogsPresenter : IDialogsPresenter
    {
        public async System.Threading.Tasks.Task<MessageBoxResult> ShowMessaeBoxYesNoCancelAsync(string message, string caption)
        {
            var mainMetroWindow = Application.Current.MainWindow as MetroWindow;
            var dialogSettings = new MetroDialogSettings() { AnimateHide = false, AnimateShow = false, AffirmativeButtonText = "Yes", NegativeButtonText = "No", FirstAuxiliaryButtonText = "Cancel", };
            var res = await mainMetroWindow.ShowMessageAsync(caption, message, MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, dialogSettings);
            switch (res)
            {
                case MessageDialogResult.Affirmative:
                    return MessageBoxResult.Yes;
                case MessageDialogResult.Negative:
                    return MessageBoxResult.No;
                case MessageDialogResult.FirstAuxiliary:
                    return MessageBoxResult.Cancel;
                default:
                    throw new ArgumentException("Invalid message box result");
            }

        }

        public async System.Threading.Tasks.Task ShowErrorAsync(string message, string caption, Type exType)
        {
            var errorMessage = $"Sorry, but an error occured: \"{message}\".\nLook more in output.";
            var mainMetroWindow = Application.Current.MainWindow as MetroWindow;
            var dialogSettings = new MetroDialogSettings() { AnimateHide = false, AnimateShow = false, AffirmativeButtonText = "OK" };
            var res = await mainMetroWindow.ShowMessageAsync(caption, errorMessage, MessageDialogStyle.Affirmative, dialogSettings);
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
