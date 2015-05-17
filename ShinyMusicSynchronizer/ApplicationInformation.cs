using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShinyMusicSynchronizer
{
    class ApplicationInformation
    {
        public string Name { get; private set; }
        public int MajorVersionNumber { get; private set; }
        public int MinorVersionNumber { get; private set; }

        public ApplicationInformation(string name, int majorVersionNumber, int minorVersionNumber)
        {
            Name = name;
            MajorVersionNumber = majorVersionNumber;
            MinorVersionNumber = minorVersionNumber;
        }
    }
}
