using IndieDevTools.Commands;

namespace IndieDevTools.States
{
    /// <summary>
    /// A state with a command player. Various commands can be added
    /// to the command player's loopable queue. Multiple queues can run
    /// in parrallel by assigning them to a layer.
    /// </summary>
    public class CommandableState : AbstractState, ICommandLayerCollection
    {
        CommandPlayer commandPlayer;
        ICommandPlayer CommandPlayer
        {
            get
            {
                if (commandPlayer == null)
                {
                    commandPlayer = new CommandPlayer();
                }
                return commandPlayer;
            }
        }

        public void AddCommand(ICommand command)
        {
            CommandPlayer.AddCommand(command);
        }

        public void AddCommand(ICommand command, int layer)
        {
            CommandPlayer.AddCommand(command, layer);
        }

        public void RemoveCommand(ICommand command)
        {
            CommandPlayer.RemoveCommand(command);
        }

        public void RemoveCommand(ICommand command, int layer)
        {
            CommandPlayer.RemoveCommand(command, layer);
        }

        public int GetLayerLoopCount(int layer)
        {
            return CommandPlayer.GetLayerLoopCount(layer);
        }

        public void SetLayerLoopCount(int layer, int loopCount)
        {
            CommandPlayer.SetLayerLoopCount(layer, loopCount);
        }

        public bool HasCommand(ICommand command)
        {
            return CommandPlayer.HasCommand(command);
        }

        public override void EnterState()
        {
            CommandPlayer.Start();
        }

        public override void ExitState()
        {
            CommandPlayer.Stop();
        }

        public override void Destroy()
        {
            CommandPlayer.Destroy();
        }

        public static CommandableState Create(string name)
        {
            CommandableState state = new CommandableState
            {
                stateName = name
            };
            return state;
        } 
    }
}