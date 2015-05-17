using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    class ConsoleMessageReporter : IMessageReporter
    {
        public void ReportMessage(ICommandMessage message)
        {
            if (message.Type == CommandMessageType.Error)
                ReportError(message.Message);
            else
                ReportInformation(message.Message);
        }

        public void ReportInformation(string message)
        {
            Console.Out.WriteLine(message);
        }

        public void ReportError(string message)
        {
            Console.Error.WriteLine(message);
        }
    }
}
