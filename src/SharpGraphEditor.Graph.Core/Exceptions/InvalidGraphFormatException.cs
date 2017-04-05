using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Exceptions
{
    public class InvalidGraphFormatException : Exception
    {
        public InvalidGraphFormatException()
        {

        }

        public InvalidGraphFormatException(string message) : base(message)
        {

        }

        public InvalidGraphFormatException(string message, Exception innerException) : base(message, innerException)
        {

        }

        protected InvalidGraphFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
