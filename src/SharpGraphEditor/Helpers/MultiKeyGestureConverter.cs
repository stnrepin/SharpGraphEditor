using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

using SharpGraphEditor.Graph.Core.Extentions;

namespace SharpGraphEditor.Helpers
{
    public class MultiKeyGestureConverter : TypeConverter
    {
        private readonly KeyConverter _keyConverter;
        private readonly ModifierKeysConverter _modifierKeysConverter;

        public MultiKeyGestureConverter()
        {
            _keyConverter = new KeyConverter();
            _modifierKeysConverter = new ModifierKeysConverter();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var keyStrokes = (value as string).Split(',');
            var firstKeyStroke = keyStrokes[0];
            var plusParts = firstKeyStroke.Split('+');

            var modifierKeys = ModifierKeys.None;
            plusParts.Take(plusParts.Length - 1).ForEach(x => modifierKeys |= (ModifierKeys)_modifierKeysConverter.ConvertFrom(x));

            var keys = new List<Key>
            {
                (Key)_keyConverter.ConvertFrom(plusParts.Last())
            };

            keyStrokes.Skip(1).ForEach(x => keys.Add((Key)_keyConverter.ConvertFrom(x)));

            return new MultiKeyGesture(keys, modifierKeys);
        }
    }
}