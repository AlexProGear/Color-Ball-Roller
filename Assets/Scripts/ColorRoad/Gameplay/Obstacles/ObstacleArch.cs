using ColorRoad.Extensions;
using ColorRoad.Systems;
using UnityEngine;
using Random = System.Random;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleArch : Obstacle
    {
        [SerializeField] private float groupHorizontalOffset = 0.4f;
        [SerializeField] private Transform ledgeLeft;
        [SerializeField] private Transform ledgeRight;
        public override ObstacleType ObstacleType => ObstacleType.InstantLose;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            Vector3 localPos = viewModel.transform.localPosition;
            var random = new Random(groupRandomSeed);
            float direction = Mathf.Sign((float)(random.NextDouble() - 0.5));
            float archCenterX = direction * groupHorizontalOffset * spline.GetPathWidth() * groupIndex / groupSize;
            localPos.x = archCenterX;
            viewModel.transform.localPosition = localPos;
            float ledgeOffset = 1.2f;
            float width = spline.GetPathWidth();
            float halfWidth = width / 2;
            float remainderLeft = halfWidth + archCenterX - ledgeOffset;
            float remainderRight = halfWidth - archCenterX - ledgeOffset;
            ledgeLeft.SetLocalPositionX(archCenterX - ledgeOffset - remainderLeft / 2);
            ledgeLeft.SetLocalScaleX(remainderLeft);
            ledgeRight.SetLocalPositionX(archCenterX + ledgeOffset + remainderRight / 2);
            ledgeRight.SetLocalScaleX(remainderRight);
        }
    }
}