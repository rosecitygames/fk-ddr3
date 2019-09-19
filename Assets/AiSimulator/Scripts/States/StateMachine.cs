using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.States
{
    /// <summary>
    /// An object that manages a collection of states and ensures
    /// that only a single state is active at a time.
    /// </summary>
    public class StateMachine : IStateMachine
    {
        public event Action<string> OnStateChange;

        protected Dictionary<string, IState> stateDictionary = new Dictionary<string, IState>();

        protected IState currentState;
        public IState CurrentState { get => currentState; }

        public IState GetState(string stateType)
        {
            IState state = null;
            if (stateDictionary.ContainsKey(stateType))
            {
                state = stateDictionary[stateType];
            }
            return state;
        }

        public void SetState(IState state)
        {
            if (stateDictionary.ContainsValue(state))
            {
                SetCurrentState(state);
            }
        }

        public void SetState(string stateName)
        {
            if (stateDictionary.ContainsKey(stateName))
            {
                IState state = stateDictionary[stateName];
                SetCurrentState(state);
            }
        }

        void SetCurrentState(IState state)
        {
            if (currentState != state)
            {
                if (currentState != null)
                {
                    currentState.ExitState();
                }
                currentState = state;
                OnStateChange?.Invoke(state.StateName);
            }
            currentState.EnterState();
        }

        public void AddState(IState state)
        {
            state.StateMachine = this;
            stateDictionary.Add(state.StateName, state);
        }

        public void RemoveState(IState state)
        {
            state.StateMachine = null;
            stateDictionary.Remove(state.StateName);
        }

        public void HandleTransition(string transitionName)
        {
            if (currentState != null)
            {
                currentState.HandleTransition(transitionName);
            }
        }

        public void Destroy()
        {
            foreach (IState state in stateDictionary.Values)
            {
                state.Destroy();
            }

            stateDictionary.Clear();
        }

        public static StateMachine Create()
        {
            return new StateMachine();
        }

    }
}