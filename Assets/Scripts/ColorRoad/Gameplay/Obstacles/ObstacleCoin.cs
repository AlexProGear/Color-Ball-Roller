using ColorRoad.Systems;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleCoin : Obstacle
    {
        [SerializeField] private float groupWidth = 0.5f;
        [SerializeField] private float movementTime = 1f;
        [SerializeField] private GameObject pickMeArrow;

        [Inject] private AchievementManager achievementManager;

        public override ObstacleType ObstacleType => ObstacleType.Coin;

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);
            viewModel.transform
                .DOLocalRotate(new Vector3(0, 360, 0), 4f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear).SetLoops(-1);
            float pathWidth = spline.GetPathWidth();
            float halfWidth = pathWidth / 2;
            float currentTimePosition = groupWidth * movementTime * groupIndex / groupSize;
            viewModel.transform
                .DOLocalMoveX(halfWidth - 1, movementTime)
                .From(1 - halfWidth)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .Goto(currentTimePosition, true);
            onPlayerCollided.AddListener(() => Destroy(gameObject));

            if (achievementManager.GetProgress(Achievement.FinishRoad) > 0 || spline.RoadNumber > 1)
                pickMeArrow.SetActive(false);
        }

        protected override void OnGameOver()
        {
            base.OnGameOver();
            DOTween.Pause(viewModel.transform);
        }

        protected override void OnRespawn()
        {
            base.OnGameOver();
            DOTween.Play(viewModel.transform);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.Kill(viewModel.transform);
        }
    }
}