using ColorRoad.Extensions;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using UnityEngine;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleEnd : Obstacle, IMessageReceiver<GenericMessage>
    {
        public override ObstacleType ObstacleType => ObstacleType.None;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            viewModel.transform.localScale = new Vector3(spline.GetPathWidth(), 1, 1);
            ApplyColor(lastSpecialColor);
            MessageBus.Subscribe(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MessageBus.Unsubscribe(this);
        }

        public override void OnMessageReceived(GenericMessage message)
        {
            base.OnMessageReceived(message);
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerColorChanged:
                    ApplyColor(message.Data.Extract<GameColor>());
                    break;
            }
        }

        private void ApplyColor(GameColor color)
        {
            SkinHelper.ApplyTrampolineSkin(GetComponentInChildren<MeshRenderer>(), color, false);
        }
    }
}