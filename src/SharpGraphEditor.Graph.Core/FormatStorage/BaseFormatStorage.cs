using System;
using System.IO;
using System.Linq;
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

            using (var stream = new StreamWriter(path, false))
            {
                try
                {
                    Save(stream, graph);
                }
                catch
                {
                    stream.BaseStream.SetLength(0);
                    stream.Flush();
                    throw;
                }
            }
        }

        public virtual void Open(TextReader reader, IGraph graph)
        {
            if (graph.Vertices.Count() > 0)
            {
                graph.Clear();
            }
        }

        public virtual void Save(TextWriter writer, IGraph graph)
        {
            
        }
    }
}
