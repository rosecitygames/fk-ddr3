namespace IndieDevTools.States
{
    /// <summary>
    /// Interface for entering and exit a state and handling transitions.
    /// </summary>
    public interface IState : IStateTransitionHandler
    {
        string StateName { get; }

        IStateMachine StateMachine { get; set; }

        void AddTransition(string transitionName, IState toState);
        void AddTransition(string transitionName, string toStateName);      
        void RemoveTransition(string transitionName);

        void EnterState();
        void ExitState();

        void Destroy();
    }
}