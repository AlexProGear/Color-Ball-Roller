using ColorRoad.Systems;
using UnityEngine;
using Random = System.Random;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleSpecialBallRow : ObstacleBallRow
    {
        [SerializeField] private float ballReplacementChance = 0.3f;
        public override ObstacleType ObstacleType => ObstacleType.SpecialBallMissed;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            if (UnityEngine.Random.Range(0f, 1f) < ballReplacementChance)
                ((ObstacleSpecialBall)balls[0]).ConvertToRegular();
        }

        protected override GameColor GetSingleBallColor(Random seededRandom, GameColor lastSpecialColor)
        {
            return lastSpecialColor;
        }

        protected override bool ForceMultipleBalls(bool isOnSpecialRoad)
        {
            return false;
        }
    }
}