namespace IndieDevTools.Commands
{
    /// <summary>
    /// A base implementation of an ICommand. At their core, commands
    /// are simply objects that can start, complete, and stop doing something.
    /// </summary>
    abstract public class AbstractCommand : ICommand
    {
        /// <summary>
        /// Whether or not the command is already completed.
        /// </summary>
        bool ICommand.IsCompleted => isCompleted;
        protected bool isCompleted;

        /// <summary>
        /// The command's parent command. All commands are nestable. Most notably,
        /// within queue enumerators.
        /// </summary>
        ICommandEnumerator ICommand.Parent
        {
            get => parent;
            set => parent = value;
        }
        protected ICommandEnumerator parent = NullCommandEnumerator.Create();

        /// <summary>
        /// Start the command.
        /// </summary>
        void ICommand.Start()
        {
            isCompleted = false;
            OnStart();
        }

        /// <summary>
        /// Stop the command. This can potentially be called before a command is completed.
        /// </summary>
        void ICommand.Stop()
        {
            OnStop();
            Complete();
        }

        /// <summary>
        /// Destroy the command. This can potentially be called before a command is completed or stopped.
        /// So, extended implementations should clean up accordingly.
        /// </summary>
        void ICommand.Destroy()
        {
            OnDestroy();
        }

        /// <summary>
        /// Complete the command
        /// </summary>
        protected virtual void Complete()
        {
            isCompleted = true;
            parent.HandleCompletedCommand(this);
        }

        /// <summary>
        /// Optionally used by extending classes when a command is started.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Optionally used by extending classes when a command is stopped.
        /// </summary>
        protected virtual void OnStop() { }

        /// <summary>
        /// Optionally used by extending classes when a command is destroyed.
        /// </summary>
        protected virtual void OnDestroy() { }

    }
}