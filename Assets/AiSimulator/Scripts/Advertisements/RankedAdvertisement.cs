using IndieDevTools.Traits;
using IndieDevTools.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Advertisements
{
    /// <summary>
    /// An advertisement with a rank property.
    /// </summary>
    public class RankedAdvertisement : IRankedAdvertisement
    {
        /// <summary>
        /// A reference to the advertisement that is being ranked.
        /// </summary>
        protected IAdvertisement Advertisement { get; set; }

        /// <summary>
        /// The arbitrary rank given to the advertisement.
        /// </summary>
        public int Rank { get; set; }

        // ITraitCollection implementations are delegated to the advertisement object.
        List<ITrait> ITraitCollection.Traits => Advertisement.Traits;
        ITrait ITraitCollection.GetTrait(string id) => Advertisement.GetTrait(id);
        void ITraitCollection.AddTrait(ITrait trait) => Advertisement.AddTrait(trait);
        void ITraitCollection.RemoveTrait(ITrait trait) => Advertisement.RemoveTrait(trait);
        void ITraitCollection.RemoveTrait(string id) => Advertisement.RemoveTrait(id);
        void ITraitCollection.Clear() => Advertisement.Clear();
        ITraitCollection ICopyable<ITraitCollection>.Copy() => Advertisement.Copy();

        // IAdvertisement implementations are delegated to the advertisement object.
        IMap IAdvertisement.Map => Advertisement.Map;
        List<Vector2Int> IAdvertisement.BroadcastLocations => Advertisement.BroadcastLocations;

        // ILocatable implementations are delegated to the advertisement object.
        Vector2Int ILocatable.Location => Advertisement.Location;
        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { } remove { } }

        // IGroupMember implementations are delegated to the advertisement object.
        int IGroupMember.GroupId { get => Advertisement.GroupId; set => Advertisement.GroupId = value; }

        /// <summary>
        /// Create a ranked advertisement.
        /// </summary>
        /// <param name="advertisement">The advertisement to be ranked</param>
        /// <param name="rank">The arbitrary rank assigned to the advertisement</param>
        /// <returns></returns>
        public static RankedAdvertisement Create(IAdvertisement advertisement, int rank)
        {
            return new RankedAdvertisement
            {
                Advertisement = advertisement,
                Rank = rank
            };
        }
    }
}

