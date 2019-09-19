using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// An implementation of ICommand that is used to create shareable command objects in the editor.
    /// </summary>
    abstract public class AbstractScriptableCommand : ScriptableObject, ICommand
    {
        bool ICommand.IsCompleted => isCompleted;
        protected bool isCompleted;

        
        ICommandEnumerator ICommand.Parent
        {
            get => parent;
            set => parent = value;
        }
        protected ICommandEnumerator parent = NullCommandEnumerator.Create();

        void ICommand.Start()
        {
            isCompleted = false;
            OnStart();
        }

        void ICommand.Stop()
        {
            OnStop();
            Complete();
        }

        void ICommand.Destroy()
        {
            OnDestroy();
        }

        protected virtual void Complete()
        {
            isCompleted = true;
            parent.HandleCompletedCommand(this);
        }

        protected virtual void OnStart() { }

        protected virtual void OnStop() { }

        protected virtual void OnDestroy() { }

    }
}