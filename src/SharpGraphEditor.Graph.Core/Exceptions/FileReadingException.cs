using System;
using System.Runtime.Serialization;

namespace SharpGraphEditor.Graph.Core.Exceptions
{

    public class FileReadingException : Exception
    {
        public FileReadingException()
        {

        }

        public FileReadingException(string message) : base(message)
        {

        }
        public FileReadingException(string message, Exception inner) : base(message, inner)
        {

        }

        protected FileReadingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
