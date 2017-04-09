using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharpGraphEditor.Graph.Core.Exceptions;

namespace SharpGraphEditor.Graph.Core.Utils
{
    public static class FormatStorageUtils
    {
        public static T ParseStringTo<T>(string stringValue)
        {

            Type typeT = typeof(T);
            try
            {
                if (typeT.IsPrimitive)
                {
                    return (T)Convert.ChangeType(stringValue, typeT, System.Globalization.CultureInfo.InvariantCulture);
                }

            }
            catch
            {

            }
            throw new InputFileFormatException($"Can't convert string \"{stringValue}\" to {typeT.Name}");
        }

        public static IEnumerable<string> ReadAllLines(TextReader stream)
        {
            var line = "";
            while ((line = stream.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public static string[] SplitByWhitespacesWithQuotes(string input)
        {
            return Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value).ToArray();
        }

        public static void PrintMatrix(TextWriter stream, int[,] matrix, int rowsCount, int columnsCount)
        {
            for (int i = 0; i < rowsCount; i++)
            {
                var line = new List<string>();
                for (int j = 0; j < columnsCount; j++)
                {
                    line.Add(matrix[i, j].ToString());
                }
                stream.WriteLine(String.Join(" ", line));
            }
        }
    }
}
