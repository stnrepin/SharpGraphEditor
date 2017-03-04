using System;
using System.Runtime.Serialization;

namespace SharpGraphEditor.Graph.Core.Exceptions
{
    public class InputFileFormatException : Exception
    {
        public InputFileFormatException() : base()
        {

        }

        public InputFileFormatException(string message) : base(message)
        {

        }

        public InputFileFormatException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected InputFileFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
