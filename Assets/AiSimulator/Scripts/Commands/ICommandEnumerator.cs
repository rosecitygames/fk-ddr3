namespace IndieDevTools.Commands
{
    /// <summary>
    /// An interface for iterating over a collection of commands.
    /// Importantly, note, that the enumerator is also a command itself
    /// which can be used for interesting nested behaviours.
    /// </summary>
    public interface ICommandEnumerator : ICommand, ICommandCollection
    {
        int LoopCount { get; set; }
        int CurrentLoop { get; }

        void HandleCompletedCommand(ICommand command);
    }
}
