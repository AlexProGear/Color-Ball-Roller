using System;
using ColorRoad.Extensions;
using ColorRoad.Gameplay.Obstacles;
using ColorRoad.Systems.Messages;
using RDG;
using Zenject;

namespace ColorRoad.Systems
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VibrationController : IInitializable, IDisposable, IMessageReceiver<GenericMessage>
    {
        [Inject] private GameSettings gameSettings;
        public void Initialize()
        {
            MessageBus.Subscribe(this);
        }

        public void Dispose()
        {
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            if (!gameSettings.VibrationEnabled.Value)
                return;
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerFollowPathBegin:
                    Vibration.Vibrate(Vibration.PredefinedEffect.EFFECT_HEAVY_CLICK);
                    break;
                case GenericMessageType.PlayerObstacleCollision:
                    switch (message.Data.Extract<Obstacle>().ObstacleType)
                    {
                        case ObstacleType.Ball:
                        case ObstacleType.BadBallsDodged:
                        case ObstacleType.Coin:
                            Vibration.Vibrate(Vibration.PredefinedEffect.EFFECT_CLICK);
                            break;
                        case ObstacleType.SpecialBall:
                            Vibration.Vibrate(Vibration.PredefinedEffect.EFFECT_HEAVY_CLICK);
                            break;
                    }
                    break;
                case GenericMessageType.GameOver:
                    Vibration.Vibrate(Vibration.PredefinedEffect.EFFECT_HEAVY_CLICK);
                    break;
            }
        }
    }
}