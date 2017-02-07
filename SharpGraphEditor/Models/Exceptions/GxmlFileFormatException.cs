using System;

namespace SharpGraphEditor.Models.Exceptions
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
