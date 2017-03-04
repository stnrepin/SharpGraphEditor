using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpGraphEditor.Controls
{
    public class Terminal : TextBox, ITerminal
    {
        public Terminal()
        {
            FontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == "Consolas").FirstOrDefault() ?? FontFamily;
            FontSize = 16;

            IsReadOnly = true;
            TextWrapping = System.Windows.TextWrapping.Wrap;
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }

        public new void Clear()
        {
            ExecuteOnUIThreadAsync(() => base.Clear());
        }

        public void WriteLine()
        {
            WriteLine(String.Empty);
        }

        public void WriteLine(string text)
        {
            Write(text + '\n');
        }

        public void Write(string text)
        {
            ExecuteOnUIThreadAsync(() =>
            {
                AppendText(text);
                ScrollToEnd();
            });
    }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        private void ExecuteOnUIThreadAsync(Action act)
        {
            Caliburn.Micro.Execute.OnUIThreadAsync(act);
        }
    }
}
