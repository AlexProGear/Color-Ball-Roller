using ColorRoad.Systems.Messages;
using DG.Tweening;
using UnityEngine;

namespace ColorRoad.Gameplay
{
    public class CameraLogic : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float sensitivityMultiplier = 0.5f;
        private Camera gameCamera;

        private void Awake()
        {
            gameCamera = GetComponent<Camera>();
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameCamera);
            MessageBus.Unsubscribe(this);
        }

        private void Update()
        {
            if (followTarget.hasChanged)
            {
                Vector3 lastLocalPosition = transform.localPosition;
                lastLocalPosition.x = followTarget.localPosition.x * sensitivityMultiplier;
                transform.localPosition = lastLocalPosition;
                followTarget.hasChanged = false;
            }
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.GameOver:
                    gameCamera.DOShakePosition(1f, 1f, 20);
                    break;
            }
        }
    }
}