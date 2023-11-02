using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using UnityEngine;
using UnityEngine.Events;

namespace ColorRoad.Gameplay.Obstacles
{
    public enum ObstacleType
    {
        None,
        Ball,
        BadBallsDodged,
        SpecialBall,
        SpecialBallMissed,
        ColorChanger,
        Coin,
        InstantLose
    }

    public abstract class Obstacle : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] protected UnityEvent onPlayerCollided;

        public abstract ObstacleType ObstacleType { get; }
        [field: SerializeField] public int RandomMinCount { get; protected set; } = 1;
        [field: SerializeField] public int RandomMaxCount { get; protected set; } = 1;
        [field: SerializeField] public float RandomInnerMinOffset { get; protected set; } = 0;
        [field: SerializeField] public float RandomInnerMaxOffset { get; protected set; } = 0;
        [SerializeField] public GameObject viewModel;

        public virtual void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            MessageBus.Subscribe(this);
        }

        protected virtual void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;
            onPlayerCollided?.Invoke();
        }

        public virtual void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.GameOver:
                    OnGameOver();
                    break;
                case GenericMessageType.Respawn:
                    OnRespawn();
                    break;
                case GenericMessageType.SkinChanged:
                    OnSkinChanged();
                    break;
            }
        }

        protected virtual void OnGameOver()
        {
        }

        protected virtual void OnRespawn()
        {
        }

        protected virtual void OnSkinChanged()
        {
        }
    }
}