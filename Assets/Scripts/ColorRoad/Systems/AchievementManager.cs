using System;
using System.Collections.Generic;
using System.Linq;
using ColorRoad.HelperClasses;
using ColorRoad.ScriptableObjects;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    public enum Achievement
    {
        AutoComplete,
        BonusBallsInARow,
        Death,
        FinishRoad,
        ReachMaxSpeed
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class AchievementManager : IInitializable, IDisposable
    {
        private Dictionary<Achievement, int> achievementsProgress = new Dictionary<Achievement, int>();
        private AchievementsInfo achievementsInfo;

        public void Initialize()
        {
            achievementsInfo = CachedResources.Get<AchievementsInfo>("Achievements Info");
            string progressJsonKeys = PlayerPrefs.GetString("achievements_progress_keys");
            string progressJsonValues = PlayerPrefs.GetString("achievements_progress_values");
            if (!progressJsonKeys.IsNullOrWhitespace())
            {
                Achievement[] keys = JsonArrayHelper.FromJson<Achievement>(progressJsonKeys);
                int[] values = JsonArrayHelper.FromJson<int>(progressJsonValues);
                achievementsProgress = new Dictionary<Achievement, int>();
                for (int i = 0; i < keys.Length; i++)
                {
                    achievementsProgress.Add(keys[i], values[i]);
                }
            }
        }

        public void Dispose()
        {
            PlayerPrefs.SetString("achievements_progress_keys", JsonArrayHelper.ToJson(achievementsProgress.Keys.ToArray()));
            PlayerPrefs.SetString("achievements_progress_values", JsonArrayHelper.ToJson(achievementsProgress.Values.ToArray()));
        }

        public void UpdateProgress(Achievement achievement, int progressValue, bool add = false)
        {
            if (!achievementsProgress.ContainsKey(achievement))
                achievementsProgress.Add(achievement, 0);

            if (add)
            {
                achievementsProgress[achievement] += progressValue;
                return;
            }

            if (progressValue > achievementsProgress[achievement])
            {
                achievementsProgress[achievement] = progressValue;
            }
        }

        public int GetProgress(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.AutoComplete:
                    return 1;
                default:
                    return achievementsProgress.TryGetValue(achievement, out int progress) ? progress : 0;
            }
        }

        public string GetAchievementText(Achievement achievement)
        {
            return achievementsInfo.GetDescription(achievement);
        }
    }
}