using System;
using ColorRoad.Extensions;
using ColorRoad.Gameplay.Obstacles;
using ColorRoad.Systems.Messages;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    public class GameManager : MonoBehaviour, IMessageReceiver<GenericMessage>, IMessageReceiver<CheatMessage>
    {
        public const float INVULNERABILITY_TIME = 3;

        [Inject] private PlayerAccount playerAccount;
        [Inject] private AchievementManager achievementManager;
        [Inject] private RoadGenerator roadGenerator;

        private int roadNumber = 0;
        private int score = 0;
        private int _pickupBonus;
        private int PickupBonus
        {
            get => _pickupBonus;
            set
            {
                _pickupBonus = value;
                achievementManager.UpdateProgress(Achievement.BonusBallsInARow, value);
            }
        }
        private GameColor currentBallColor;
        private bool godMode;

        private void Awake()
        {
            MessageBus.Subscribe((IMessageReceiver<GenericMessage>)this);
            MessageBus.Subscribe((IMessageReceiver<CheatMessage>)this);
            SkinHelper.UpdateGameColorPalette();
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe((IMessageReceiver<GenericMessage>)this);
            MessageBus.Unsubscribe((IMessageReceiver<CheatMessage>)this);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            SetPlayerColor(GameColors.ActiveColors.GetRandom());
            roadGenerator.Initialize();
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerFollowPathBegin:
                    roadNumber++;
                    MessageBus.Post(new GenericMessage(GenericMessageType.NewRoad, roadNumber));
                    MessageBus.Post(new AudioMessage(AudioMessageType.NewRoad));
                    MessageBus.Post(new VFXMessage(VFXMessageType.Landing));
                    break;
                case GenericMessageType.PlayerFollowPathEnd:
                    achievementManager.UpdateProgress(Achievement.FinishRoad, roadNumber);
                    break;
                case GenericMessageType.PlayerObstacleCollision:
                    OnObstacleCollision(message.Data.Extract<Obstacle>());
                    break;
                case GenericMessageType.PlayerColorChanged:
                    var color = message.Data.Extract<GameColor>();
                    SkinHelper.ApplyEnvironmentColors(color);
                    MessageBus.Post(new AudioMessage(AudioMessageType.ChangeColor,
                        GameColors.ActiveColors.IndexOf(color)));
                    break;
                case GenericMessageType.PauseGame:
                    Time.timeScale = 0;
                    break;
                case GenericMessageType.ContinueGame:
                    Time.timeScale = 1;
                    break;
                case GenericMessageType.Respawn:
                    godMode = true;
                    UniTask.Delay(TimeSpan.FromSeconds(INVULNERABILITY_TIME)).GetAwaiter()
                        .OnCompleted(() => godMode = false);
                    break;
                case GenericMessageType.ReturnToMainMenu:
                    bool isFinished = message.Data.Extract<bool>();
                    MessageBus.Post(new GenericMessage(GenericMessageType.FinalScore, isFinished, score, roadNumber));
                    break;
            }
        }

        public void OnMessageReceived(CheatMessage message)
        {
            switch (message.CheatType)
            {
                case Cheat.GodMode:
                    godMode = message.Data.Extract<bool>();
                    break;
            }
        }

        private void OnObstacleCollision(Obstacle obstacle)
        {
            bool isSpecial = false;
            switch (obstacle.ObstacleType)
            {
                case ObstacleType.SpecialBallMissed:
                    PickupBonus = 0;
                    break;
                case ObstacleType.SpecialBall:
                    if (((ObstacleBall)obstacle).color == currentBallColor)
                    {
                        PickupBonus++;
                        MessageBus.Post(new AudioMessage(AudioMessageType.BallPickupSpecial,
                            GameColors.ActiveColors.IndexOf(currentBallColor)));
                    }

                    isSpecial = true;
                    goto case ObstacleType.Ball;
                case ObstacleType.Ball:
                    if (((ObstacleBall)obstacle).color != currentBallColor)
                    {
                        goto case ObstacleType.InstantLose;
                    }

                    int addedScore = PickupBonus + 1;
                    score += addedScore;
                    MessageBus.Post(new GenericMessage(GenericMessageType.ScoreChanged, score));
                    MessageBus.Post(new GenericMessage(GenericMessageType.BallPickup, addedScore, obstacle));
                    MessageBus.Post(new VFXMessage(VFXMessageType.PickupBall));
                    if (!isSpecial)
                        MessageBus.Post(new AudioMessage(AudioMessageType.BallPickup,
                            GameColors.ActiveColors.IndexOf(currentBallColor)));
                    break;
                case ObstacleType.ColorChanger:
                    SetPlayerColor(((ObstacleColorChanger)obstacle).color);
                    MessageBus.Post(new VFXMessage(VFXMessageType.ChangeColor));
                    break;
                case ObstacleType.Coin:
                    int addedCoins = 1;
                    playerAccount.Coins.Value += addedCoins;
                    MessageBus.Post(new GenericMessage(GenericMessageType.CoinPickup, addedCoins, obstacle));
                    MessageBus.Post(new AudioMessage(AudioMessageType.Click));
                    MessageBus.Post(new VFXMessage(VFXMessageType.PickupBall));
                    break;
                case ObstacleType.InstantLose:
                    if (!godMode)
                        OnGameOver();
                    break;
            }
        }

        private void SetPlayerColor(GameColor color)
        {
            currentBallColor = color;
            MessageBus.Post(new GenericMessage(GenericMessageType.PlayerColorChanged, currentBallColor));
        }

        private void OnGameOver()
        {
            achievementManager.UpdateProgress(Achievement.Death, 1, true);
            MessageBus.Post(new GenericMessage(GenericMessageType.GameOver, score, roadNumber));
            MessageBus.Post(new AudioMessage(AudioMessageType.Death));
            MessageBus.Post(new VFXMessage(VFXMessageType.Death));
        }
    }
}