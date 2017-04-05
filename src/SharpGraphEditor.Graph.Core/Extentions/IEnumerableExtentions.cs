using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Extentions
{
    public static class IEnumerableExtentions
    {
        public static string ToReadableString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder("[ ");
            sb.Append(String.Join(", ", enumerable));
            sb.Append(" ]");
            return sb.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action should not be null");
            }

            foreach (var el in enumerable)
            {
                action(el);
            }
        }

        public static void For<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            int i = 0;
            foreach (var e in enumerable)
            {
                action(e, i);
                i++;
            }
        }

        public static IEnumerable<int> AllIndexesOf<T>(this IEnumerable<T> enumerable, Func<T, bool> func)
        {
            int i = 0;
            foreach (var e in enumerable)
            {
                if (func(e))
                {
                    yield return i;
                }
                i++;
            }
        }
    }
}
