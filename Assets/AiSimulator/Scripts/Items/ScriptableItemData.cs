using IndieDevTools.Advertisements;
using IndieDevTools.Traits;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Items
{
    /// <summary>
    /// Allows items to be setup with a shared data object in the editor.
    /// Note, runtime changes to the data do not effect the serialized data.
    /// </summary>
    [CreateAssetMenu(fileName = "ItemData", menuName = "IndieDevTools/Item Data")]
    public class ScriptableItemData : ScriptableObject, IItemData
    {
        [SerializeField]
        ItemData data = null;
        IItemData runtimeData = null;
        IItemData RuntimeData
        {
            get
            {
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying == false) return data;
#endif
                if (runtimeData == null)
                {
                    if (data == null)
                    {
                        runtimeData = NullItemData.Create();
                    }
                    else
                    {
                        runtimeData = (data as IItemData).Copy();
                    }
                }

                return runtimeData;
            }
        }

        string IDescribable.DisplayName
        {
            get => RuntimeData.DisplayName;
            set => RuntimeData.DisplayName = value;
        }

        string IDescribable.Description
        {
            get => RuntimeData.Description;
            set => RuntimeData.Description = value;
        }

        event Action<IDescribable> IUpdatable<IDescribable>.OnUpdated
        {
            add { RuntimeData.OnUpdated += value; }
            remove { RuntimeData.OnUpdated -= value; }
        }

        float IAdvertisementBroadcastData.BroadcastDistance => RuntimeData.BroadcastDistance;

        float IAdvertisementBroadcastData.BroadcastInterval => RuntimeData.BroadcastInterval;

        List<ITrait> IStatsCollection.Stats => RuntimeData.Stats;
        ITrait IStatsCollection.GetStat(string id) => RuntimeData.GetStat(id);

        IItemData ICopyable<IItemData>.Copy() => RuntimeData.Copy();
    }
}
