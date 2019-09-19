using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// An abstract enumerator for iterating over a collection of commands.
    /// </summary>
    public class AbstractCommandEnumerator : AbstractCommand, ICommandEnumerator
    {
        protected List<ICommand> commands = new List<ICommand>();
        protected virtual int CommandsCount => commands.Count;

        protected bool isActive = false;

        int ICommandEnumerator.LoopCount
        {
            get => LoopCount;
            set => LoopCount = value;
        }
        protected virtual int LoopCount { get; set; }

        int ICommandEnumerator.CurrentLoop => CurrentLoop;
        protected virtual int CurrentLoop { get; set; }

        void ICommandCollection.AddCommand(ICommand command) => AddCommand(command);
        protected virtual void AddCommand(ICommand command)
        {
            commands.Add(command);
            command.Parent = this;
        }

        void ICommandCollection.RemoveCommand(ICommand command) => RemoveCommand(command);
        protected virtual void RemoveCommand(ICommand command)
        {
            command.Stop();
            commands.Remove(command);
        }

        bool ICommandCollection.HasCommand(ICommand command) => HasCommand(command);
        protected virtual bool HasCommand(ICommand command)
        {
            int commandIndex = commands.IndexOf(command);
            return commandIndex >= 0;
        }

        void ICommandEnumerator.HandleCompletedCommand(ICommand command) => HandleCompletedCommand(command);
        protected virtual void HandleCompletedCommand(ICommand command) { }

        protected bool GetIsCommandCompleted(ICommand command) => command.IsCompleted;
    }
}
