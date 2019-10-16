using System;
using UnityEngine;

namespace IndieDevTools.Traits
{
    /// <summary>
    /// A class used to define traits with unique quantities.
    /// The scriptable object property is used for shared property referencing of the trait.
    /// Note, quantity changes at runtime DO effect serialization. So, this class is ideal for save data.
    /// </summary>
    [Serializable]
    public class Trait : ITrait
    {
        [SerializeField]
        ScriptableTrait trait = null;
        ITrait data;
        ITrait Data
        {
            get
            {
                InitData();
                return data;
            }
        }

        void InitData()
        {
            if (data == null)
            {
                if (trait == null)
                {
                    data = new NullTrait();
                }
                else
                {
                    data = trait;
                }
            }
        }

        string IIdable.Id => Data.Id;

        [SerializeField]
        int quantity;
        int ITrait.Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                int newValue = Mathf.Clamp(value, Min, Max);
                if (quantity != newValue)
                {
                    quantity = newValue;
                    OnUpdated?.Invoke(this);
                }
            }
        }

        int initialQuantity;

        int ITrait.Min { get => Min; set => Min = value; }
        int Min
        {
            get
            {
                if (isOverridingMin) return overrideMin;
                return Data.Min;
            }
            set
            {
                isOverridingMin = true;
                overrideMin = value;
            }
        }

        bool isOverridingMin = false;
        int overrideMin = 0;

        int ITrait.Max { get => Max; set => Max = value; }
        int Max
        {
            get
            {
                if (isOverridingMax) return overrideMax;
                if (Data.IsInitialMax) return initialQuantity;
                return Data.Max;
            }
            set
            {
                isOverridingMax = true;
                overrideMax = value;
            }
        }

        bool isOverridingMax = false;
        int overrideMax = 0;

        bool ITrait.IsInitialMax => Data.IsInitialMax;

        string IDescribable.DisplayName { get => Data.DisplayName; set => Data.DisplayName = value; }
        string IDescribable.Description { get => Data.Description; set => Data.Description = value; }
        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated { add { (Data as IDescribable).OnUpdated += value; } remove { (Data as IDescribable).OnUpdated -= value; } }

        event Action<ITrait> IUpdatable<ITrait>.OnUpdated { add { OnUpdated += value; } remove { OnUpdated -= value; } }
        Action<ITrait> OnUpdated;

        ITrait ICopyable<ITrait>.Copy()
        {
            return Create(this, quantity);
        }

        public static ITrait Create(ITrait source, int quantity = 0)
        {
            return new Trait()
            {
                data = source,
                quantity = quantity,
                initialQuantity = quantity
            };
        }
    }
}
