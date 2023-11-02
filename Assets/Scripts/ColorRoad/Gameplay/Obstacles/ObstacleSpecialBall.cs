using ColorRoad.Systems;
using UnityEngine;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleSpecialBall : ObstacleBall
    {
        private ObstacleType obstacleType = ObstacleType.SpecialBall;
        public override ObstacleType ObstacleType => obstacleType;

        protected override void ApplyMaterial()
        {
            SkinHelper.ApplyBallSkin(meshRenderer, color, true);
        }

        public void ConvertToRegular()
        {
            base.ApplyMaterial();
            obstacleType = ObstacleType.Ball;
        }
    }
}