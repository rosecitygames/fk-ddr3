using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// An extension of the abstract command enumerator that plays through
    /// commands in parallel. All commands in the collection run at the same time
    /// until all are completed. Note, that not all commands necessarily complete.
    /// So, in those cases, the enumerator will never complete (which can be desirable).
    /// </summary>
    public class ParallelCommandEnumerator : AbstractCommandEnumerator
    {
        override protected void OnStart()
        {
            CurrentLoop = 0;
            isCompleted = false;
            isActive = true;
            commands.ForEach(StartCommand);
        }
        protected void StartCommand(ICommand command)
        {
            command.Start();
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
            commands.ForEach(DestoryCommand);
            commands.Clear();
        }
        protected void DestoryCommand(ICommand command)
        {
            command.Destroy();
        }

        override protected void HandleCompletedCommand(ICommand command)
        {
            if (isCompleted == false)
            {
                int index = commands.IndexOf(command);
                if (index >= 0)
                {
                    bool isAllCommandsCompleted = commands.TrueForAll(GetIsCommandCompleted);
                    if (isAllCommandsCompleted)
                    {
                        bool isLoopsRemaining = CurrentLoop < LoopCount - 1;
                        bool isInfiniteLooping = LoopCount < 0;
                        if (isLoopsRemaining || isInfiniteLooping)
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
        }

    }
}