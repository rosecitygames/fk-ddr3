using System.Collections.Generic;

namespace IndieDevTools.Commands
{
    /// <summary>
    /// A class that is used to create a loopable queue of commands. Optionally, multiple
    /// queues can be run in parrallel by assigning commands to a queue layer.
    /// 
    /// Very similar to an animation timeline with multiple layers where the frames are commands.
    /// It can also be loosely thought of as a two dimensional array off commands.
    /// 
    /// Note, the command player is also a command itself. So, this can be used for interesting
    /// nested behaviours.
    /// </summary>
    public class CommandPlayer : AbstractCommand, ICommandPlayer
    {
        private ICommandEnumerator parallelCommandEnumerator;
        private ICommandEnumerator ParallelCommandEnumerator
        {
            get
            {
                if (parallelCommandEnumerator == null)
                {
                    parallelCommandEnumerator = new ParallelCommandEnumerator();
                    parallelCommandEnumerator.Parent = this;
                }
                return parallelCommandEnumerator;
            }
        }

        protected List<ICommandEnumerator> layers = new List<ICommandEnumerator>();
       
        int ICommandEnumerator.LoopCount
        {
            get => loopCount;
            set => loopCount = value;
        }
        protected int loopCount;

        int ICommandEnumerator.CurrentLoop => currentLoop;
        protected int currentLoop = 0;

        public int LayersCount => layers.Count;

        void ICommandCollection.AddCommand(ICommand command) => AddCommand(command, -1);
        void ICommandLayerCollection.AddCommand(ICommand command, int layer) => AddCommand(command, layer);
        void AddCommand(ICommand command, int layerIndex)
        {
            if (layerIndex < 0)
            {
                AddCommandToLastLayer(command);
            }
            else
            {
                while (LayersCount <= layerIndex)
                {
                    AddLayer();
                }

                ICommandEnumerator layer = layers[layerIndex];
                layer.AddCommand(command);
            }      
        }

        void AddCommandToLastLayer(ICommand command)
        {
            if (LayersCount <= 0)
            {
                AddLayer();
            }

            int layerIndex = LayersCount - 1;
            ICommandEnumerator layer = layers[layerIndex];
            layer.AddCommand(command);
        }

        void AddLayer()
        {
            ICommandEnumerator layer = new SerialCommandEnumerator();
            ParallelCommandEnumerator.AddCommand(layer);
            layers.Add(layer);
        }

        void ICommandCollection.RemoveCommand(ICommand command)
        {
            foreach (ICommandEnumerator layer in layers)
            {
                bool hasCommand = layer.HasCommand(command);
                if (hasCommand)
                {
                    layer.RemoveCommand(command);
                    break;
                }
            }
        }

        void ICommandLayerCollection.RemoveCommand(ICommand command, int layerIndex)
        {
            if (ContainsLayer(layerIndex))
            {
                ICommandEnumerator layer = layers[layerIndex];
                layer.RemoveCommand(command);
            }
        }

        bool ICommandCollection.HasCommand(ICommand command)
        {
            foreach (ICommandEnumerator layer in layers)
            {
                bool hasCommand = layer.HasCommand(command);
                if (hasCommand)
                {
                    return true;
                }
            }

            return false;
        }

        int ICommandLayerCollection.GetLayerLoopCount(int layerIndex)
        {
            if (ContainsLayer(layerIndex))
            {
                ICommandEnumerator layer = layers[layerIndex];
                return layer.LoopCount;
            }
            return 0;
        }

        void ICommandLayerCollection.SetLayerLoopCount(int layerIndex, int loopCount)
        {
            if (ContainsLayer(layerIndex))
            {
                ICommandEnumerator layer = layers[layerIndex];
                layer.LoopCount = loopCount;
            }
        }

        bool ContainsLayer(int layerIndex)
        {
            return (layerIndex >= 0 && layerIndex < layers.Count);
        }

        public ICommandEnumerator CreateLayer() => CreateLayer(0);
        public ICommandEnumerator CreateLayer(int loopCount)
        {
            ICommandEnumerator layer = new SerialCommandEnumerator();
            layer.LoopCount = loopCount;
            ParallelCommandEnumerator.AddCommand(layer);
            layers.Add(layer);
            return layer;
        }

        public void DestroyLayer(int layer)
        {
            if (layer < LayersCount)
            {
                ICommandEnumerator selectedLayer = layers[layer];
                ParallelCommandEnumerator.RemoveCommand(selectedLayer);
                layers.Remove(selectedLayer);
                selectedLayer.Destroy();
            }
        }

        public void HandleCompletedCommand(ICommand command)
        {
            if (isCompleted == false)
            {
                if (command == ParallelCommandEnumerator)
                {
                    if (currentLoop <= 0 && loopCount < 0)
                    {
                        OnStart();
                    }
                    else if (currentLoop < loopCount - 1)
                    {
                        int nextLoop = currentLoop + 1;
                        OnStart();
                        currentLoop = nextLoop;
                    }
                    else
                    {
                        Complete();
                    }
                }
            }
        }

        override protected void OnStart()
        {
            currentLoop = 0;
            isCompleted = false;
            ParallelCommandEnumerator.Start();
        }

        override protected void OnStop()
        {
            ParallelCommandEnumerator.Stop();
        }

        override protected void OnDestroy()
        {
            ParallelCommandEnumerator.Destroy();
            if (layers != null)
            {
                layers.Clear();
                layers = null;
            }
        }
    }
}