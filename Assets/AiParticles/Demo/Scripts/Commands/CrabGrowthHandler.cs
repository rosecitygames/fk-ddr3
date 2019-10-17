using IndieDevTools.Commands;
using IndieDevTools.Demo.BattleSimulator;
using IndieDevTools.Traits;

namespace IndieDevTools.Demo.CrabBattle
{
    public class CrabGrowthHandler : AbstractCommand
    {
        ICrab crab = null;

        string onExplodeTransition;
        string onMoltTransition;

        const int maxSize = 235;
        int currentSize = 0;

        bool isInitialUpdateCompleted = false;

        protected override void OnStart()
        {
            AddEventHandler();
        }

        protected override void OnStop()
        {
            RemoveEventHandler();
        }

        protected override void OnDestroy()
        {
            RemoveEventHandler();
        }

        void AddEventHandler()
        {
            RemoveEventHandler();
            ITrait sizeTrait = (crab as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);
            if (sizeTrait == null) return;
            (sizeTrait as IUpdatable<ITrait>).OnUpdated += OnSizeTraitUpdated;
        }

        void RemoveEventHandler()
        {
            ITrait sizeTrait = (crab as IStatsCollection).GetStat(TraitsUtil.sizeTraitId);
            if (sizeTrait == null) return;
            (sizeTrait as IUpdatable<ITrait>).OnUpdated -= OnSizeTraitUpdated;
        }

        void OnSizeTraitUpdated(ITrait sizeTrait)
        {
            if (isInitialUpdateCompleted == false)
            {
                isInitialUpdateCompleted = true;
                return;
            }

            if (sizeTrait.Quantity >= maxSize)
            {
                crab.HandleTransition(onExplodeTransition);
            }
            else if (sizeTrait.Quantity > currentSize && currentSize > 0)
            {
                crab.HandleTransition(onMoltTransition);
            }
            else
            {
                currentSize = sizeTrait.Quantity;
            }
        }

        public static ICommand Create(ICrab crab, string onExplodeTransition, string onMoltTransition)
        {
            return new CrabGrowthHandler
            {
                crab = crab,
                onExplodeTransition = onExplodeTransition,
                onMoltTransition = onMoltTransition
            };
        }
    }
}
