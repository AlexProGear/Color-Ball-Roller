using System;
using ColorRoad.Extensions;
using ColorRoad.ScriptableObjects;
using ColorRoad.Systems.Messages;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems.Audio
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AudioManager : IInitializable, IDisposable, IMessageReceiver<AudioMessage>
    {
        [Inject] private GameSettings gameSettings;

        private GameSoundsData gameSoundsData;

        private const float MaxBallSoundDeltaTime = 10f;

        private int lastColorIndex;
        private int lastBallIndex = 0;
        private float lastBallTimestamp;

        private GameObject audioSources;

        public void Initialize()
        {
            gameSoundsData = CachedResources.Get<GameSoundsData>("GameSoundsData");
            MessageBus.Subscribe(this);
            audioSources = new GameObject("Audio Sources");
        }

        public void Dispose()
        {
            MessageBus.Unsubscribe(this);
            MonoBehaviour.Destroy(audioSources);
        }

        public void OnMessageReceived(AudioMessage message)
        {
            if (!gameSettings.SoundEnabled.Value)
                return;
            int colorIndex;
            switch (message.MessageType)
            {
                case AudioMessageType.BallPickup:
                    colorIndex = message.Data.Extract<int>();
                    PlayBallAudio(colorIndex);
                    break;
                case AudioMessageType.BallPickupSpecial:
                    colorIndex = message.Data.Extract<int>();
                    if (colorIndex != lastColorIndex)
                        break;
                    InstantAudioSource.Create(audioSources, gameSoundsData.pickupBallSpecial[lastColorIndex]);
                    break;
                case AudioMessageType.ChangeColor:
                    colorIndex = message.Data.Extract<int>();
                    lastColorIndex = colorIndex;
                    lastBallIndex = 0;
                    InstantAudioSource.Create(audioSources, gameSoundsData.changeColor[lastColorIndex]);
                    break;
                case AudioMessageType.Death:
                    InstantAudioSource.Create(audioSources, gameSoundsData.death.GetRandom());
                    break;
                case AudioMessageType.NewRoad:
                    InstantAudioSource.Create(audioSources, gameSoundsData.newRoad.GetRandom());
                    break;
                case AudioMessageType.Click:
                    InstantAudioSource.Create(audioSources, gameSoundsData.coin.GetRandom());
                    break;
            }
        }

        private void PlayBallAudio(int colorIndex)
        {
            if (colorIndex != lastColorIndex)
                return;

            AudioClip[] audioClips = colorIndex switch
            {
                0 => gameSoundsData.pickupBall1,
                1 => gameSoundsData.pickupBall2,
                2 => gameSoundsData.pickupBall3,
                _ => gameSoundsData.pickupBall1
            };

            InstantAudioSource.Create(audioSources, audioClips[lastBallIndex]);

            float deltaTime = Time.time - lastBallTimestamp;

            if (deltaTime > MaxBallSoundDeltaTime)
                lastBallIndex = 0;
            else
                lastBallIndex = Mathf.Min(lastBallIndex + 1, audioClips.Length - 1);

            lastBallTimestamp = Time.time;
        }
    }
}