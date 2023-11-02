using System;
using ColorRoad.Extensions;
using ColorRoad.Systems.Messages;
using UniRx;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class PlayerAccount : IInitializable, IDisposable, IMessageReceiver<GenericMessage>
    {
        public ReactiveProperty<int> Coins;
        public ReactiveProperty<int> BestRoad;
        public ReactiveProperty<int> BestScore;

        public void Initialize()
        {
            Coins.Value = PlayerPrefs.GetInt("coins", 0);
            BestRoad.Value = PlayerPrefs.GetInt("best_road", 0);
            BestScore.Value = PlayerPrefs.GetInt("best_score", 0);
            MessageBus.Subscribe(this);
        }

        public void Dispose()
        {
            PlayerPrefs.SetInt("coins", Coins.Value);
            PlayerPrefs.SetInt("best_road", BestRoad.Value);
            PlayerPrefs.SetInt("best_score", BestScore.Value);
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.GameOver:
                    int score = message.Data.Extract<int>(0);
                    int road = message.Data.Extract<int>(1);
                    ApplyHighscore(score, road);
                    break;
            }
        }

        private void ApplyHighscore(int score, int road)
        {
            if (score > BestScore.Value)
            {
                BestScore.Value = score;
                MessageBus.Post(new GenericMessage(GenericMessageType.NewBestScore, score));
            }

            if (road > BestRoad.Value)
            {
                BestRoad.Value = road;
                MessageBus.Post(new GenericMessage(GenericMessageType.NewBestRoad, road));
            }
        }
    }
}