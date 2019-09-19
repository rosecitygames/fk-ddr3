namespace IndieDevTools.Commands
{
    /// <summary>
    /// An interface for a collection of commands that includes
    /// helper methods for altering the command list.
    /// </summary>
    public interface ICommandCollection
    {
        void AddCommand(ICommand command);
        void RemoveCommand(ICommand command);
        bool HasCommand(ICommand command);
    }
}