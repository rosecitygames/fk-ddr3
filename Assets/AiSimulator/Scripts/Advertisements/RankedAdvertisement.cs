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
        protected IAdvertisement Advertisement { get; set; }
        public int Rank { get; set; }

        List<ITrait> ITraitCollection.Traits => Advertisement.Traits;
        ITrait ITraitCollection.GetTrait(string id) => Advertisement.GetTrait(id);
        void ITraitCollection.AddTrait(ITrait trait) => Advertisement.AddTrait(trait);
        void ITraitCollection.RemoveTrait(ITrait trait) => Advertisement.RemoveTrait(trait);
        void ITraitCollection.RemoveTrait(string id) => Advertisement.RemoveTrait(id);
        void ITraitCollection.Clear() => Advertisement.Clear();
        ITraitCollection ICopyable<ITraitCollection>.Copy() => Advertisement.Copy();

        IMap IAdvertisement.Map => Advertisement.Map;
        List<Vector2Int> IAdvertisement.BroadcastLocations => Advertisement.BroadcastLocations;

        Vector2Int ILocatable.Location => Advertisement.Location;
        event Action<ILocatable> IUpdatable<ILocatable>.OnUpdated { add { } remove { } }

        int IGroupMember.GroupId { get => Advertisement.GroupId; set => Advertisement.GroupId = value; }

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

