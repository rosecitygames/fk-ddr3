using System;

namespace IndieDevTools.States
{
    /// <summary>
    /// An interface for an object that can be in single state amongst a collection of states.
    /// </summary>
    public interface IStateMachine : IStateTransitionHandler
    {
        event Action<string> OnStateChange;

        IState CurrentState { get; }

        IState GetState(string stateName);
        void SetState(string stateName);
        void SetState(IState state);

        void AddState(IState state);
        void RemoveState(IState state);

        void Destroy();
    }
}