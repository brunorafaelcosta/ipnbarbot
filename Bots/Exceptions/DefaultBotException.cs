using System;

namespace ipnbarbot.Bots.Exceptions
{
    public class DefaultBotException : Exception
    {
        public DefaultBotException(string message) : base (message)
        {
        }
    }
}