using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    interface IMessageReporter
    {
        void ReportMessage(ICommandMessage message);

        void ReportInformation(string message);

        void ReportError(string message);
    }
}
