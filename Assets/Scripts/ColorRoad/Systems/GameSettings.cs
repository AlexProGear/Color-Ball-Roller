using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    public class GameSettings : IInitializable, IDisposable
    {
        public ReactiveProperty<bool> VibrationEnabled;
        public ReactiveProperty<bool> SoundEnabled;

        public void Initialize()
        {
            VibrationEnabled.Value = PlayerPrefs.GetInt("vibration", 1) == 1;
            SoundEnabled.Value = PlayerPrefs.GetInt("sound", 1) == 1;
        }

        public void Dispose()
        {
            PlayerPrefs.SetInt("vibration", VibrationEnabled.Value ? 1 : 0);
            PlayerPrefs.SetInt("sound", SoundEnabled.Value ? 1 : 0);
        }
    }
}