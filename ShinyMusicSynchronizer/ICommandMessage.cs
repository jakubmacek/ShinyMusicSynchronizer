namespace ShinyMusicSynchronizer
{
    internal interface ICommandMessage
    {
        string Message { get; }
        CommandMessageType Type { get; }
    }
}