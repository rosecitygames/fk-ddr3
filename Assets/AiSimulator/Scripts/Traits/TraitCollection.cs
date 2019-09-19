using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A collection of traits that includes implemenation of several helper methods
    /// for altering the list.
    /// </summary>
    [Serializable]
    public class TraitCollection : ITraitCollection
    {
        [SerializeField]
        List<Trait> traits = new List<Trait>();
        List<ITrait> ITraitCollection.Traits
        {
            get
            {
                return Collection;
            }
        }
        List<ITrait> collection = null;
        List<ITrait> Collection
        {
            get
            {
                if (collection == null)
                {
                    collection = new List<ITrait>();
                    foreach (ITrait trait in traits)
                    {
                        collection.Add(trait);
                    }
                }
                return collection;
            }
        }

        ITrait ITraitCollection.GetTrait(string id)
        {
            return GetTrait(id);
        }
        ITrait GetTrait(string id)
        {
            return Collection.Find(trait => trait.Id == id);
        }

        void ITraitCollection.AddTrait(ITrait value)
        {
            if (value == null) return;

            ITrait trait = GetTrait(value.Id);
            bool hasTrait = trait != null;
            if (hasTrait == false)
            {
                Collection.Add(trait);
            }
        }

        void ITraitCollection.RemoveTrait(ITrait value)
        {
            if (value == null) return;
            RemoveTrait(value.Id);
        }

        void ITraitCollection.RemoveTrait(string id) => RemoveTrait(id);
        void RemoveTrait(string id)
        {
            ITrait trait = GetTrait(id);
            bool hasTrait = trait != null;
            if (hasTrait)
            {
                Collection.Remove(trait);
            }
        }

        void ITraitCollection.Clear()
        {
            Collection.Clear();
        }

        ITraitCollection ICopyable<ITraitCollection>.Copy()
        {
            ITraitCollection copy = new TraitCollection();
            foreach(ITrait trait in Collection)
            {
                copy.AddTrait(trait.Copy());
            }
            return copy;
        }

        public static ITraitCollection Create(ITraitCollection source)
        {
            TraitCollection traitCollection = new TraitCollection();
            foreach (ITrait trait in source.Traits)
            {
                traitCollection.Collection.Add(trait.Copy());
            }
            return traitCollection;
        }

        public static ITraitCollection Create(List<ITrait> source)
        {
            TraitCollection traitCollection = new TraitCollection();
            foreach (ITrait trait in source)
            {
                traitCollection.Collection.Add(trait.Copy());
            }
            return traitCollection;
        }

        public static ITraitCollection Create()
        {
            return new TraitCollection();
        }
    }
}
