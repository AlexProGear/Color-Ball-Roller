using System;
using ColorRoad.Systems;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace ColorRoad.Gameplay.Obstacles
{
    public class ObstacleBall : Obstacle
    {
        [SerializeField] private GameObject pickMeArrow;

        [Inject] private AchievementManager achievementManager;
        
        public override ObstacleType ObstacleType => ObstacleType.Ball;

        [NonSerialized] public GameColor color;
        protected MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = viewModel.GetComponent<MeshRenderer>();
        }

        public override void Initialize(SplineLogic spline, bool isOnSpecialRoad, ref GameColor lastSpecialColor,
            int groupRandomSeed, int groupIndex, int groupSize)
        {
            base.Initialize(spline, isOnSpecialRoad, ref lastSpecialColor, groupRandomSeed, groupIndex, groupSize);

            ApplyMaterial();

            if (lastSpecialColor != color || achievementManager.GetProgress(Achievement.FinishRoad) > 0 || spline.RoadNumber > 1)
                pickMeArrow.SetActive(false);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (!other.CompareTag("Player"))
                return;
            if (other.GetComponentInParent<PlayerLogic>().Color == color)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void ApplyMaterial()
        {
            SkinHelper.ApplyBallSkin(meshRenderer, color, false);
        }

        protected override void OnGameOver()
        {
            base.OnGameOver();
            DOTween.Pause(transform);
        }

        protected override void OnRespawn()
        {
            base.OnGameOver();
            DOTween.Play(transform);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.Kill(transform);
        }

        protected override void OnSkinChanged()
        {
            color = GameColors.GetNewColorAfterReskin(color);
            ApplyMaterial();
        }
    }
}