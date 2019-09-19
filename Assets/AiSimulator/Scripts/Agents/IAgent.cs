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
        IAgentData Data { get; set; }

        event Action<IAdvertisement> OnAdvertisementReceived;

        IRankedAdvertisement TargetAdvertisement { get; set; }
        IMapElement TargetMapElement { get; set; }
        Vector2Int TargetLocation { get; set; }
    }
}

