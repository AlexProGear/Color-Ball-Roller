using System.Collections.Generic;
using ColorRoad.Systems;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace ColorRoad.ScriptableObjects
{
    [CreateAssetMenu(order = 4)]
    public class AchievementsInfo : SerializedScriptableObject
    {
        [OdinSerialize] private Dictionary<Achievement, string> descriptions;
        public string GetDescription(Achievement achievement)
        {
            return descriptions[achievement];
        }
    }
}