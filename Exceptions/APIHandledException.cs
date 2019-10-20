using System;

namespace ipnbarbot.Exceptions
{
    public class APIHandledException : Exception
    {
        public APIHandledException(string message) : base (message)
        {
        }
    }
}