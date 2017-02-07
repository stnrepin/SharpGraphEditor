using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGraphEditor.Graph.Core.Exceptions
{
    public class GxmlFileFormatException : Exception
    {
        public GxmlFileFormatException() : base()
        {

        }

        public GxmlFileFormatException(string message) : base(message)
        {

        }

        public GxmlFileFormatException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
