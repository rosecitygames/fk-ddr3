using IndieDevTools.Traits;
using IndieDevTools.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// A class used by advertisement broadcasters to announce that a collection of
    /// traits are at a location.
    /// </summary>
    public class Advertisement : IAdvertisement
    {
        // ITraitCollection implementations
        List<ITrait> ITraitCollection.Traits => Traits.Traits;
        ITrait ITraitCollection.GetTrait(string id) => Traits.GetTrait(id);
        void ITraitCollection.AddTrait(ITrait trait) => Traits.AddTrait(trait);
        void ITraitCollection.RemoveTrait(ITrait trait) => Traits.RemoveTrait(trait);
        void ITraitCollection.RemoveTrait(string id) => Traits.RemoveTrait(id);
        void ITraitCollection.Clear() => Traits.Clear();
        ITraitCollection ICopyable<ITraitCollection>.Copy() => Traits.Copy();
        protected ITraitCollection Traits { get; set; }

        // ILocatable implementations
        Vector2Int ILocatable.Location => Location;
        protected Vector2Int Location { get; set; }

        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { } remove { } }

        // IAdvertisement implementations
        List<Vector2Int> IAdvertisement.BroadcastLocations => BroadcastLocations;
        protected List<Vector2Int> BroadcastLocations { get; set; }

        // IAdvertisement implementations
        IMap IAdvertisement.Map => Map;
        protected IMap Map { get; set; }

        // IGroupMember implementations
        int IGroupMember.GroupId { get => GroupId; set => GroupId = value; }
        protected int GroupId { get; set; }

        /// <summary>
        /// Create an advertisement.
        /// </summary>
        /// <param name="traits">Traits that will be broadcasted</param>
        /// <param name="map">The map that the advertisement will be broadcasted to</param>
        /// <param name="location">The location of the advertisement</param>
        /// <param name="broadcastLocations">Locations that the advertisement will be broadcasted to</param>
        /// <param name="groupId">The group Id associated with the advertisement</param>
        /// <returns></returns>
        public static IAdvertisement Create(List<ITrait> traits, IMap map, Vector2Int location, List<Vector2Int> broadcastLocations, int groupId = 0)
        {
            Advertisement advertisement = new Advertisement
            {
                Traits = TraitCollection.Create(traits),
                Map = map,
                Location = location,
                BroadcastLocations = broadcastLocations,
                GroupId = groupId
            };
            return advertisement;
        }
    }
}
