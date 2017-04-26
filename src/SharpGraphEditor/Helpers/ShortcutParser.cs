using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using System.Windows.Input;

using SharpGraphEditor.Behaviors;
using System.Globalization;

namespace SharpGraphEditor.Helpers
{
    public static class ShortcutParser
    {
        public static bool CanParse(string triggerText)
        {
            return !string.IsNullOrWhiteSpace(triggerText) && triggerText.Contains("Shortcut");
        }

        public static TriggerBase CreateTrigger(string triggerText)
        {
            var triggerDetail = triggerText
                .Replace("[", String.Empty)
                .Replace("]", String.Empty)
                .Replace("Shortcut", String.Empty)
                .Trim();

            var gestureConverter = new MultiKeyGestureConverter();
            var gesture = gestureConverter.ConvertFrom(null, CultureInfo.InvariantCulture, triggerDetail) as MultiKeyGesture;
            var trigger = new InputBindingTrigger { InputBinding = new MultiKeyBinding() { Gesture = gesture } };
            return trigger;
        }

    }
}
