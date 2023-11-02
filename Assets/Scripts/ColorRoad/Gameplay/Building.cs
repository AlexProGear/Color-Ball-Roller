using ColorRoad.Extensions;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using UnityEngine;

namespace ColorRoad.Gameplay
{
    public class Building : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private Transform view;
        [SerializeField] private LayerMask selfDestructMask;

        private void Start()
        {
            if (IsOverlappingRoad())
            {
                Destroy(gameObject);
                return;
            }

            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private bool IsOverlappingRoad()
        {
            return Physics.CheckBox(view.position, view.lossyScale / 2, view.rotation, selfDestructMask);
        }

        public void ApplyColor(GameColor color)
        {
            var mesh = GetComponentInChildren<MeshRenderer>();
            SkinHelper.ApplyBuildingSkin(mesh, color);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerColorChanged:
                    ApplyColor(message.Data.Extract<GameColor>());
                    break;
            }
        }
    }
}