using System;
using UnityEngine;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A scriptable trait that can be shared across various objects.
    /// Note, trait quantity is not implemented and any runtime changes
    /// do not effect the serialized data.
    /// </summary>
    [CreateAssetMenu(fileName = "Trait", menuName = "IndieDevTools/Trait")]
    public class ScriptableTrait : ScriptableObject, ITrait
    {
        string IIdable.Id => name;

        [SerializeField, Tooltip("The displayed name for this trait")]
        string displayName = "";
        [NonSerialized]
        string runtimeDisplayName = "";
        string IDescribable.DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(runtimeDisplayName))
                {
                    runtimeDisplayName = displayName;
                }
                return runtimeDisplayName;
            }

            set
            {
                if (runtimeDisplayName != value)
                {
                    runtimeDisplayName = value;
                    OnDescribableUpdated.Invoke(this);
                }
            }
        }

        [SerializeField, TextArea, Tooltip("A description of this trait")]
        string description = "";
        [NonSerialized]
        string runtimeDescription = "";
        string IDescribable.Description
        {
            get
            {
                if (string.IsNullOrEmpty(runtimeDescription))
                {
                    runtimeDescription = description;
                }
                return runtimeDescription;
            }

            set
            {
                if (runtimeDescription != value)
                {
                    runtimeDescription = value;
                    OnDescribableUpdated.Invoke(this);
                }
            }
        }

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { OnDescribableUpdated += value; } remove { OnDescribableUpdated -= value; } }
        Action<IDescribable> OnDescribableUpdated;

        [SerializeField, Tooltip("Whether or not the initial value set is the max value")]
        bool isInitialMax = false;
        bool ITrait.IsInitialMax => isInitialMax;

        [SerializeField, Tooltip("The minimum quanitity value for this trait")]
        int min = 0;
        int ITrait.Min { get => min; set { } }

        [SerializeField, Tooltip("The maximum quanitity value for this trait")]
        int max = 99;
        int ITrait.Max { get => max; set { } }

        int ITrait.Quantity { get => 0; set { } }

        event Action<ITrait> IUpdatable<ITrait>.OnUpdated { add { } remove { } }

        ITrait ICopyable<ITrait>.Copy()
        {
            return Trait.Create(this);
        }
    }
}
