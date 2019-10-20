using System;

namespace ipnbarbot.Exceptions
{
    public class ApplicationHandledException : Exception
    {
        public ApplicationHandledException(string message) : base (message)
        {
        }
    }
}