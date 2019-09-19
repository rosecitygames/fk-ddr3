namespace IndieDevTools.States
{
    /// <summary>
    /// An interface for an object that can handle state transitions.
    /// </summary>
    public interface IStateTransitionHandler
    {
        void HandleTransition(string transitionName);
    }
}

