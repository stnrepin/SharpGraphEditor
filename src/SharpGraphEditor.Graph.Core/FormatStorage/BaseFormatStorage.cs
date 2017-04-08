using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using SharpGraphEditor.Graph.Core.Exceptions;

namespace SharpGraphEditor.Graph.Core.FormatStorage
{
    public abstract class BaseFormatStorage
    {
        public void Open(string path, IGraph graph)
        {
            try
            {
                using (var fileStream = File.OpenRead(path))
                {
                    using (var stream = new StreamReader(fileStream))
                    {
                        Open(stream, graph);
                    }
                }
            }
            catch (InputFileFormatException)
            {
                graph.Clear();
                throw;
            }
            catch (Exception ex)
            {
                graph.Clear();
                throw new FileReadingException("During reading of text file an error occured", ex);
            }
        }

        public void Save(string path, IGraph graph)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                try
                {
                    fileStream.SetLength(0);
                    fileStream.Flush();
                    using (var stream = new StreamWriter(fileStream))
                    {
                        Save(stream, graph);
                    }
                }
                catch
                {
                    ClearStream(fileStream);
                    throw;
                }
            }
        }

        public abstract void Open(TextReader stream, IGraph graph);

        public abstract void Save(TextWriter stream, IGraph graph);

        private void ClearStream(Stream stream)
        {
            stream.SetLength(0);
            stream.Flush();
        }

        protected T ParseStringTo<T>(String stringValue)
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

        protected static IEnumerable<string> ReadAllLines(TextReader stream)
        {
            var line = "";
            while ((line = stream.ReadLine()) != null)
            {
                yield return line;
            }
        }

    }
}
