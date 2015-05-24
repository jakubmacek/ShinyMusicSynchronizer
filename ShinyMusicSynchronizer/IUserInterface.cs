using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    interface IUserInterface
    {
        void ReportMessage(ICommandMessage message);

        void ReportInformation(string message);

        void ReportError(string message);

        bool AskConfirmation(string question);
    }
}
