namespace IndieDevTools.States
{
    /// <summary>
    /// A simple named state.
    /// </summary>
    public class SimpleState : AbstractState
    {
        public static IState Create(string name)
        {
            IState state = new SimpleState
            {
                stateName = name
            };

            return state;
        }
    }
}