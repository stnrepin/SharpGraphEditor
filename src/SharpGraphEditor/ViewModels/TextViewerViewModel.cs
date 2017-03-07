using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;

namespace SharpGraphEditor.ViewModels
{
    public class TextViewerViewModel : PropertyChangedBase
    {
        private string _text;
        private bool _canCopy;
        private bool _canCancel;
        private bool _isReadOnly;

        public TextViewerViewModel(string text, bool canCopy, bool canCancel, bool isReadOnly)
        { 
            Text = text;
            CanCopy = canCopy;
            CanCancel = canCancel;
            IsReadOnly = isReadOnly;
        }

        public void Ok(IClose closableWindow)
        {
            closableWindow.TryClose(true);
        }

        public void Cancel(IClose closableWindow)
        {
            closableWindow.TryClose(false);
        }

        public void CopyText()
        {
            Clipboard.SetText(Text);
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        public bool CanCopy
        {
            get { return _canCopy; }
            set
            {
                _canCopy = value;
                NotifyOfPropertyChange(() => CanCopy);
            }
        }

        public bool CanCancel
        {
            get { return _canCancel; }
            set
            {
                _canCancel = value;
                NotifyOfPropertyChange(() => CanCancel);
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                NotifyOfPropertyChange(() => IsReadOnly);
            }
        }
    }
}
