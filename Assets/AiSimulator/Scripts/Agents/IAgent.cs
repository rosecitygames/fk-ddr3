using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using IndieDevTools.Maps;
using IndieDevTools.States;
using System;
using UnityEngine;

namespace IndieDevTools.Agents
{
    /// <summary>
    /// An interface for agents that contains agent data, target properties, and an advertisement received event.
    /// </summary>
    public interface IAgent : IAdvertisingMapElement, IAdvertisementReceiver, IStateTransitionHandler, IDesiresCollection
    {
        /// <summary>
        /// The agent's data object.
        /// </summary>
        IAgentData Data { get; set; }

        /// <summary>
        /// An event that gets invoked when an advertisement is received.
        /// </summary>
        event Action<IAdvertisement> OnAdvertisementReceived;

        /// <summary>
        /// The agent's target advertisement.
        /// </summary>
        IRankedAdvertisement TargetAdvertisement { get; set; }

        /// <summary>
        /// The agent's target map element.
        /// </summary>
        IMapElement TargetMapElement { get; set; }

        /// <summary>
        /// The agent's target location.
        /// </summary>
        Vector2Int TargetLocation { get; set; }
    }
}

