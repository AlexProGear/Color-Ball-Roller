using ColorRoad.Systems.Messages;
using UnityEngine;

namespace ColorRoad.Gameplay
{
    public class BallShadowLogic : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private Transform ball;

        private const float FLOOR_OFFSET = 0.1f;

        private void Awake()
        {
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private void Update()
        {
            transform.position = ball.position - ball.up * (ball.localPosition.y - FLOOR_OFFSET);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerFollowPathBegin:
                    gameObject.SetActive(true);
                    break;
                case GenericMessageType.PlayerFollowPathEnd:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }
}