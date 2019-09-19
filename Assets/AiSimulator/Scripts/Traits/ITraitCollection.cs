using System.Collections.Generic;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// An interface for a generic collection of traits.
    /// </summary>
    public interface ITraitCollection : ICopyable<ITraitCollection>
    {
        List<ITrait> Traits { get; }
        ITrait GetTrait(string id);

        void AddTrait(ITrait trait);
        void RemoveTrait(ITrait trait);
        void RemoveTrait(string id);
        void Clear();
    }
}
