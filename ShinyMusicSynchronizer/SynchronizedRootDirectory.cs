namespace ShinyMusicSynchronizer
{
    class SynchronizedRootDirectory : SynchronizedDirectory
    {
        public SynchronizedRootDirectory()
        {
            Parent = null;
            Name = "";
            Path = "";
            IsOnComputer = true;
            IsOnDevice = true;
        }
    }
}