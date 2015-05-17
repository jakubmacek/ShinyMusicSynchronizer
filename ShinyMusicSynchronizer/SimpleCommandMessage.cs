using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    class SimpleCommandMessage : ICommandMessage
    {
        public string Message { get; private set; }
        public CommandMessageType Type { get; private set; }

        //public SimpleCommandMessage(string message)
        //    : this(message, CommandMessageType.Information)
        //{
        //}

        public SimpleCommandMessage(string message, CommandMessageType type)
        {
            Contract.Assert(message != null);
            Message = message;
            Type = type;
        }
    }
}
