using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// An extension of the abstract command enumerator that iterates
    /// through its collection of commands in a serial fashion (one after the other)
    /// until all commands are completed.
    /// </summary>
    public class SerialCommandEnumerator : AbstractCommandEnumerator
    {

        protected int currentIndex = 0;

        protected ICommand CurrentCommand
        {
            get
            {
                if (currentIndex < 0 || currentIndex > commands.Count - 1) return NullCommand.Create();
                return commands[currentIndex];
            }
        }

        override protected void OnStart()
        {
            currentIndex = 0;
            CurrentLoop = 0;
            isCompleted = false;
            isActive = true;

            if (CommandsCount > 0)
            {
                CurrentCommand.Start();
            }
        }

        override protected void OnStop()
        {
            isActive = false;
            commands.ForEach(StopCommand);
            Complete();
        }
        protected void StopCommand(ICommand command)
        {
            command.Stop();
        }

        override protected void OnDestroy()
        {
            commands.ForEach(DestroyCommand);
            commands.Clear();
            currentIndex = 0;
            CurrentLoop = 0;
        }
        protected void DestroyCommand(ICommand command)
        {
            command.Destroy();
        }

        override protected void HandleCompletedCommand(ICommand command)
        {
            if (isActive && isCompleted == false && command == CurrentCommand)
            {
                StartNextCommand();
            }
        }

        protected void StartNextCommand()
        {
            bool isCommandsRemaining = currentIndex < CommandsCount - 1;
            bool isLoopsRemaining = CurrentLoop < LoopCount;
            bool isInfiniteLooping = LoopCount < 0;

            if (isCommandsRemaining)
            {
                currentIndex += 1;
                CurrentCommand.Start();
            }
            else if (isLoopsRemaining || isInfiniteLooping)
            {
                int nextLoop = CurrentLoop + 1;
                OnStart();
                CurrentLoop = nextLoop;
            }
            else
            {
                Complete();
            }
        }
    }
}