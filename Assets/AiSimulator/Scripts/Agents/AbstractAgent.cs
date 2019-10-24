using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using IndieDevTools.Maps;
using IndieDevTools.States;
using IndieDevTools.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Agents
{
    /// <summary>
    /// An abstract map element that advertises its stat traits, seeks out desired traits, and can change states.
    /// </summary>
    public abstract class AbstractAgent : MonoBehaviour, IAgent
    {
        protected const int sortingOrderOffset = 1000;

        // IAgentData implementations
        [SerializeField]
        ScriptableAgentData data = null;
        IAgentData iData;
        IAgentData IAgent.Data { get => Data; set => Data = value; }
        protected IAgentData Data
        {
            get
            {
                InitData();
                return iData;
            }
            set
            {
                iData = value;
            }
        }

        protected virtual void InitData()
        {
            if (iData == null)
            {
                if (data == null)
                {
                    iData = new NullAgentData();
                }
                else
                {
                    iData = (data as IAgentData).Copy();
                }
            }  
        }

        // Group Member implementations
        [SerializeField]
        int groupId;
        protected int GroupId { get => groupId; set => groupId = value; }
        int IGroupMember.GroupId { get => GroupId; set => GroupId = value; }

        string IDescribable.DisplayName { get => DisplayName; set => DisplayName = value; }
        protected virtual string DisplayName { get => Data.DisplayName; set => Data.DisplayName = value; }

        string IDescribable.Description { get => Description; set => Description = value; }
        protected virtual string Description { get => Data.Description; set => Data.Description = value; }

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { Data.OnUpdated += value; } remove { Data.OnUpdated -= value; } }

        List<ITrait> IStatsCollection.Stats => Data.Stats;
        ITrait IStatsCollection.GetStat(string id) => Data.GetStat(id);

        List<ITrait> IDesiresCollection.Desires => Data.Desires;
        ITrait IDesiresCollection.GetDesire(string id) => Data.GetDesire(id);

        // IMapElement implementations
        protected IMapElement MapElement
        {
            get
            {
                InitMapElement();
                return mapElement;
            }
        }
        [NonSerialized]
        IMapElement mapElement = null;

        protected void InitMapElement()
        {
            if (mapElement == null)
            {
                mapElement = Maps.MapElementHelper.Create(gameObject, this, sortingOrderOffset);
                mapElement.AddToMap();
            }
        }

        IMap IMapElement.Map { get => MapElement.Map; set => MapElement.Map = value; }

        void IMapElement.AddToMap() => AddToMap();
        protected virtual void AddToMap() => MapElement.AddToMap();

        void IMapElement.AddToMap(IMap map) => AddToMap(map);
        protected virtual void AddToMap(IMap map) => MapElement.AddToMap(map);

        void IMapElement.RemoveFromMap() => RemoveFromMap();
        protected virtual void RemoveFromMap() => MapElement.RemoveFromMap();

        bool IMapElement.IsOnMap => IsOnMap;
        protected virtual bool IsOnMap => MapElement.IsOnMap;

        float IMapElement.Distance(IMapElement otherMapElement) => Distance(otherMapElement);
        protected virtual float Distance(IMapElement otherMapElement) => MapElement.Distance(otherMapElement);

        float IMapElement.Distance(Vector2Int otherLocation) => Distance(otherLocation);
        protected virtual float Distance(Vector2Int otherLocation) => MapElement.Distance(otherLocation);

        int IMapElement.InstanceId => MapElement.InstanceId;

        int IMapElement.SortingOrder => SortingOrder;
        protected virtual int SortingOrder => MapElement.SortingOrder;

        // ILocatable implementations
        Vector2Int ILocatable.Location => Location;
        protected virtual Vector2Int Location => MapElement.Location;

        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated
        {
            add { (MapElement as IUpdatable<ILocatable>).OnUpdated += value; }
            remove { (MapElement as IUpdatable<ILocatable>).OnUpdated -= value; }
        }

        // IPositionable implementations
        Vector3 IPositionable.Position { get => Position; set => Position = value; }
        protected virtual Vector3 Position { get => MapElement.Position; set => MapElement.Position = value; }

        // IAdvertisementBroadcastData implementations
        float IAdvertisementBroadcastData.BroadcastDistance => BroadcastDistance;
        protected float BroadcastDistance => Data.BroadcastDistance;

        float IAdvertisementBroadcastData.BroadcastInterval => BroadcastInterval;
        protected float BroadcastInterval => Data.BroadcastInterval;

        IAdvertiser advertiser = null;
        protected IAdvertiser Advertiser
        {
            get
            {
                if (advertiser == null)
                {
                    InitAdvertiser();
                }
                return advertiser;
            }
            set
            {
                advertiser = value;
            }
        }

        protected virtual void InitAdvertiser()
        {
            InitBroadcaster();
            advertiser = Advertisements.Advertiser.Create(broadcaster);
        }

        [SerializeField]
        ScriptableAdvertisementBroadcaster broadcaster = null;

        void InitBroadcaster()
        {
            if (broadcaster != null)
            {
                (broadcaster as IAdvertisementBroadcaster).AddReceiver(this);
            }
        }

        // IAdvertiser implementations
        IAdvertisementBroadcaster IAdvertiser.GetBroadcaster()
        {
            return advertiser.GetBroadcaster();
        }

        void IAdvertiser.SetBroadcaster(IAdvertisementBroadcaster broadcaster)
        {
            advertiser.SetBroadcaster(broadcaster);
        }

        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement)
        {
            advertiser.BroadcastAdvertisement(advertisement);
        }

        void IAdvertiser.BroadcastAdvertisement(IAdvertisement advertisement, IAdvertisementReceiver excludeReceiver)
        {
            advertiser.BroadcastAdvertisement(advertisement, excludeReceiver);
        }

        // IAdvertisementReceiver implementations
        void IAdvertisementReceiver.ReceiveAdvertisement(IAdvertisement advertisement)
        {
            OnAdvertisementReceived?.Invoke(advertisement);
        }

        event Action<IAdvertisement> IAgent.OnAdvertisementReceived
        {
            add
            {
                OnAdvertisementReceived += value;
            }
            remove
            {
                OnAdvertisementReceived -= value;
            }
        }

        Action<IAdvertisement> OnAdvertisementReceived;

        // IAgent implementations
        IRankedAdvertisement IAgent.TargetAdvertisement { get; set; }

        IMapElement IAgent.TargetMapElement { get; set; }

        Vector2Int IAgent.TargetLocation { get; set; }

        // State Machine implementations
        protected IStateMachine stateMachine = StateMachine.Create();

        protected virtual void InitStateMachine() { }

        void IStateTransitionHandler.HandleTransition(string transitionName) => HandleTransition(transitionName);
        protected void HandleTransition(string transitionName)
        {
            stateMachine.HandleTransition(transitionName);
        }

        // Initialization
        void Start() => Init();

        protected virtual void Init()
        {
            InitData();
            InitAdvertiser();
            InitMapElement();
            InitStateMachine();
        }

        // Cleanup
        void OnDestroy() => Cleanup();

        protected virtual void Cleanup()
        {
            MapElement.RemoveFromMap();

            IAdvertisementBroadcaster advertisementBroadcaster = Advertiser.GetBroadcaster();
            if (advertisementBroadcaster != null)
            {
                advertisementBroadcaster.RemoveReceiver(this);
            }

            if (stateMachine != null)
            {
                stateMachine.Destroy();
            }
        }

        [SerializeField]
        bool isDrawingGizmos = true;

        [SerializeField]
        Color gizmoColor = Color.green;

        protected bool isDrawingRuntimeGizmos = true;

        void OnDrawGizmos()
        {
            if (isDrawingGizmos == false) return;
            if (isDrawingRuntimeGizmos == false) return;

            IAgent agent = this as IAgent;
            if (agent.TargetAdvertisement != null)
            {
                DrawGizmosUtil.DrawTargetAdvertisementLocationLine(this, Color.blue);
            }

            if (data != null && MapElement.Map != null)
            {
                float broadcastDistance = (data as IAgentData).BroadcastDistance * MapElement.Map.CellSize.x;
                DrawGizmosUtil.DrawBroadcastDistanceSphere(MapElement.Position, broadcastDistance, gizmoColor);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (isDrawingGizmos == false) return;
            if (isDrawingRuntimeGizmos == false) return;
            if (data == null || MapElement.Map == null) return;

            float broadcastDistance = (data as IAgentData).BroadcastDistance * MapElement.Map.CellSize.x;
            DrawGizmosUtil.DrawBroadcastDistanceWireSphere(MapElement.Position, broadcastDistance, gizmoColor);
        }
    }
}
