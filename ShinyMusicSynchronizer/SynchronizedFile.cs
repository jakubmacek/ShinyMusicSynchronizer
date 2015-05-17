using System.IO;
using PortableDeviceLib;
using PortableDeviceLib.Model;

namespace ShinyMusicSynchronizer
{
    class SynchronizedFile : SynchronizedItem
    {
        public long SizeOnDevice { get; set; }
        public long SizeOnComputer { get; set; }

        public PortableDeviceFileObject ObjectOnDevice { get; set; }
        public FileInfo ObjectOnComputer { get; set; }
    }
}