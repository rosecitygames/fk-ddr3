using System.Collections.Generic;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A themed collection used to define desired traits.
    /// </summary>
    public interface IDesiresCollection
    {
        List<ITrait> Desires { get; }
        ITrait GetDesire(string id);
    }
}
