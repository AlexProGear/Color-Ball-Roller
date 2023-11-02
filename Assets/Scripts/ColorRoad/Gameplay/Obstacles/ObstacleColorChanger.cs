using ColorRoad.Extensions;
using ColorRoad.Systems;
using ModestTree;
using UnityEngine;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleColorChanger : Obstacle
    {
        public override ObstacleType ObstacleType => ObstacleType.ColorChanger;
        public GameColor color;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);

            viewModel.transform.localScale = new Vector3(spline.GetPathWidth(), 1, 1);
            color = GameColors.ActiveColors.GetScrambled().Except(lastSpecialColor).GetRandom();
            lastSpecialColor = color;
            SkinHelper.ApplyTrampolineSkin(GetComponentInChildren<MeshRenderer>(), color, true);
        }

        protected override void OnSkinChanged()
        {
            color = GameColors.GetNewColorAfterReskin(color);
            SkinHelper.ApplyTrampolineSkin(GetComponentInChildren<MeshRenderer>(), color, true);
        }
    }
}