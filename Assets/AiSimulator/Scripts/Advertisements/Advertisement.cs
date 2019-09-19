using IndieDevTools.Traits;
using IndieDevTools.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// A class used by advertisement broadcasters to announce that a collection of traits are at a location.
    /// </summary>
    public class Advertisement : IAdvertisement
    {
        protected ITraitCollection Traits { get; set; }
        List<ITrait> ITraitCollection.Traits => Traits.Traits;
        ITrait ITraitCollection.GetTrait(string id) => Traits.GetTrait(id);
        void ITraitCollection.AddTrait(ITrait trait) => Traits.AddTrait(trait);
        void ITraitCollection.RemoveTrait(ITrait trait) => Traits.RemoveTrait(trait);
        void ITraitCollection.RemoveTrait(string id) => Traits.RemoveTrait(id);
        void ITraitCollection.Clear() => Traits.Clear();
        ITraitCollection ICopyable<ITraitCollection>.Copy() => Traits.Copy();

        Vector2Int ILocatable.Location => Location;
        protected Vector2Int Location { get; set; }

        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { } remove { } }

        List<Vector2Int> IAdvertisement.BroadcastLocations => BroadcastLocations;
        protected List<Vector2Int> BroadcastLocations { get; set; }

        IMap IAdvertisement.Map => Map;
        protected IMap Map { get; set; }

        int IGroupMember.GroupId { get => GroupId; set => GroupId = value; }
        protected int GroupId { get; set; }
  
        public static IAdvertisement Create(List<ITrait> attributes, IMap map, Vector2Int location, List<Vector2Int> broadcastLocations, int groupId = 0)
        {
            Advertisement advertisement = new Advertisement
            {
                Traits = TraitCollection.Create(attributes),
                Map = map,
                Location = location,
                BroadcastLocations = broadcastLocations,
                GroupId = groupId
            };
            return advertisement;
        }
    }
}
