using IndieDevTools.Advertisements;
using IndieDevTools.Agents;
using IndieDevTools.Traits;
using IndieDevTools.Commands;
using IndieDevTools.States;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.BattleSimulator
{
    /// <summary>
    /// A command that handles incoming advertisements received by agents.
    /// If advertisement is desired then its set as the agents target.
    /// If an agent target already exists, then switch to a new target if
    /// the incoming advertisement ranks higher.
    /// </summary>
    public class AdvertisementHandler : AbstractCommand
    {
        IAgent agent = null;
        string targetFoundTransition;

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
            agent.OnAdvertisementReceived += HandleAdvertisement;
        }

        void RemoveEventHandler()
        {
            agent.OnAdvertisementReceived -= HandleAdvertisement;
        }

        void HandleAdvertisement(IAdvertisement advertisement)
        {
            int rank = GetAdvertisementRank(advertisement);
            if (rank > 0)
            {
                bool isRankGreater = GetIsAdRankGreaterThanTarget(rank);
                if (isRankGreater)
                {
                    agent.TargetAdvertisement = RankedAdvertisement.Create(advertisement, rank);
                    agent.TargetLocation = advertisement.Location;

                    if (string.IsNullOrEmpty(targetFoundTransition) == false)
                    {
                        agent.HandleTransition(targetFoundTransition);
                    }
                }
            }       
        }

        int GetAdvertisementRank(IAdvertisement advertisement)
        {
            if (advertisement.GroupId == agent.GroupId) return 0;

            List<ITrait> desires = agent.Desires;
            List<ITrait> ads = advertisement.Traits;

            int rank = 0;
            foreach (ITrait attribute in ads)
            {
                foreach (ITrait desire in desires)
                {
                    if (attribute.Id == desire.Id)
                    {
                        int attributeRank = attribute.Quantity * desire.Quantity;
                        rank += attributeRank;
                    }
                }
            }
            return rank;
        }

        bool GetIsAdRankGreaterThanTarget(int rank)
        {
            bool isRankGreater = false;
            if (rank > 0)
            {
                bool hasTargetAdvertisement = agent.TargetAdvertisement != null;
                if (hasTargetAdvertisement)
                {
                    if (rank > agent.TargetAdvertisement.Rank)
                    {
                        isRankGreater = true;
                    }
                }
                else
                {
                    isRankGreater = true;
                }
            }

            return isRankGreater;
        }

        public static ICommand Create(IAgent agent, string completedTransition = "")
        {
            return new AdvertisementHandler
            {
                agent = agent,
                targetFoundTransition = completedTransition
            };
        }
    }
}
