using System.Collections.Generic;
using System.IO;
using PortableDeviceLib;
using PortableDeviceLib.Model;

namespace ShinyMusicSynchronizer
{
    class SynchronizedDirectory : SynchronizedItem
    {
        public List<SynchronizedDirectory> Directories { get; set; }
        public List<SynchronizedFile> Files { get; set; }

        public PortableDeviceContainerObject ObjectOnDevice { get; set; }
        public DirectoryInfo ObjectOnComputer { get; set; }

        public SynchronizedDirectory()
        {
            Directories = new List<SynchronizedDirectory>();
            Files = new List<SynchronizedFile>();
        }
    }
}