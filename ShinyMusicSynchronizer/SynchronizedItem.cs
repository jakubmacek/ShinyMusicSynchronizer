using System;
using PortableDeviceLib;

namespace ShinyMusicSynchronizer
{
    abstract class SynchronizedItem
    {
        public SynchronizedDirectory Parent { get; set; }

        public string Name { get; set; }
        public string Path { get; set; }

        public bool IsOnDevice { get; set; }
        public bool IsOnComputer { get; set; }

        public SynchronizationOperation Operation
        {
            get
            {
                if (IsOnComputer)
                {
                    if (IsOnDevice)
                        return SynchronizationOperation.Keep;
                    else
                        return SynchronizationOperation.Copy;
                }
                else
                {
                    if (IsOnDevice)
                        return SynchronizationOperation.Delete;
                    else
                        return SynchronizationOperation.Nothing;
                }
            }
        }

        public void ComputePath()
        {
            Path = Name;
            if (Parent != null && !string.IsNullOrEmpty(Parent.Path))
                Path = Parent.Path + "/" + Path;
        }
    }
}