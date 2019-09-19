using System;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A null implementation of a trait.
    /// </summary>
    [Serializable]
    public class NullTrait : ITrait
    {
        string IDescribable.DisplayName { get => ""; set { } }
        string IDescribable.Description { get => ""; set { } }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { } remove { } }

        string IIdable.Id { get => ""; }
        int ITrait.Quantity { get => 0; set { } }
        int ITrait.Min { get => 0; set { } }
        int ITrait.Max { get => 0; set { } }
        bool ITrait.IsInitialMax => false;

        event Action<ITrait> IUpdatable<ITrait>.OnUpdated { add { } remove { } }

        ITrait ICopyable<ITrait>.Copy() => new NullTrait();

        public static ITrait Create() => new NullTrait();
    }
}
