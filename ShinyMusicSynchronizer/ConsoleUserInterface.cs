using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    class ConsoleUserInterface : IUserInterface
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

        public bool AskConfirmation(string question)
        {
            Console.Out.WriteLine(question);
            Console.Out.Write("[Y/N] ");
            var key = Console.ReadKey();
            Console.Out.WriteLine();
            if ((key.KeyChar == 'y') || (key.KeyChar == 'Y'))
                return true;
            else
                return false;
        }
    }
}
