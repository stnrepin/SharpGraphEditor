using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SharpGraphEditor.Controls
{
    public class Terminal : TextBox, ITerminal
    {
        public DependencyProperty ShowOnWriteProperty =
            DependencyProperty.Register("ShowOnWrite", typeof(bool), typeof(Terminal),
                new FrameworkPropertyMetadata(false));

        public bool ShowOnWrite
        {
            get { return (bool)base.GetValue(ShowOnWriteProperty); }
            set { base.SetValue(ShowOnWriteProperty, value); }
        }

        public Terminal()
        {
            FontFamily = Fonts.SystemFontFamilies.Where(x => x.Source == "Consolas").FirstOrDefault() ?? FontFamily;
            FontSize = 16;

            IsReadOnly = true;
            TextWrapping = TextWrapping.Wrap;
            VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
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

                if (ShowOnWrite)
                {
                    Visibility = System.Windows.Visibility.Visible;
                }
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
