using System.Linq;
using ColorRoad.Extensions;
using ColorRoad.Systems;
using DG.Tweening;
using ModestTree;
using Sirenix.Utilities;
using UnityEngine;
using Random = System.Random;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleBallRow : Obstacle
    {
        [SerializeField] protected ObstacleBall[] balls;
        [SerializeField] private float singleBallChance = 0.4f;
        [SerializeField] private float singleBallMovementTime = 1f;

        public override ObstacleType ObstacleType => ObstacleType.BadBallsDodged;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            var seededRandom = new Random(groupRandomSeed);
            int ballCount = isOnSpecialRoad ? 2 : 3;
            if (!ForceMultipleBalls(isOnSpecialRoad) && seededRandom.NextDouble() <= singleBallChance)
            {
                // Один шарик в ряду
                balls.ForEach(ball => ball.gameObject.SetActive(false));
                ObstacleBall singleBall = balls[0];
                singleBall.gameObject.SetActive(true);
                singleBall.color = GetSingleBallColor(seededRandom, lastSpecialColor);
                float roadWidth = spline.GetPathWidth() / 2;
                singleBall.transform
                    .DOLocalMoveX(roadWidth - 1, singleBallMovementTime)
                    .From(1 - roadWidth)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .Goto(UnityEngine.Random.Range(0, singleBallMovementTime), true);
            }
            else
            {
                // Множество шариков в ряду
                GameColor[] randomColors = GameColors.ActiveColors.GetScrambled().ToArray();
                if (isOnSpecialRoad)
                    randomColors = randomColors.Except(lastSpecialColor).ToArray();
                for (int i = 0; i < ballCount; i++)
                {
                    balls[i].color = randomColors[i];
                }

                if (isOnSpecialRoad)
                {
                    balls.Take(2).ForEach(ball => ball.gameObject.SetActive(true));
                    balls.Skip(2).ForEach(ball => ball.gameObject.SetActive(false));
                }
            }

            float pathWidth = spline.GetPathWidth() - 2f;
            float offset = pathWidth / ((isOnSpecialRoad ? 4 : 3) - 1);
            if (!isOnSpecialRoad)
            {
                for (int i = 0; i < ballCount; i++)
                {
                    float horizontalPosition = offset * i - pathWidth / 2;
                    balls[i].transform.localPosition = Vector3.right * horizontalPosition;
                }
            }
            else
            {
                int[] positions = Enumerable.Range(0, 4).GetScrambled().Take(2).ToArray();
                for (int i = 0; i < ballCount; i++)
                {
                    float horizontalPosition = offset * positions[i] - pathWidth / 2;
                    balls[i].transform.localPosition = Vector3.right * horizontalPosition;
                }
            }

            foreach (ObstacleBall ball in balls)
            {
                ball.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            }
        }

        protected virtual GameColor GetSingleBallColor(Random seededRandom, GameColor lastSpecialColor)
        {
            return GameColors.ActiveColors.GetRandom(seededRandom);
        }

        protected virtual bool ForceMultipleBalls(bool isOnSpecialRoad)
        {
            return isOnSpecialRoad;
        }
    }
}