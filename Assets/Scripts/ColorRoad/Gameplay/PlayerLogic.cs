using ColorRoad.Extensions;
using ColorRoad.Gameplay.Obstacles;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace ColorRoad.Gameplay
{
    [RequireComponent(typeof(SplineFollower))]
    public class PlayerLogic : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject viewModel;
        [SerializeField] private GameObject shadow;
        [SerializeField] private float longJumpHeight = 10;
        [SerializeField] private float ballHopHeight = 1.5f;
        [SerializeField, BoxGroup("Speed particles")]
        private ParticleSystem speedParticles;
        [SerializeField, BoxGroup("Speed particles")]
        private float minCompletionTime = 15;
        [SerializeField, BoxGroup("Speed particles")]
        private float maxCompletionTime = 21;
        [SerializeField] private Transform tailPivot;
        [SerializeField] private Material whiteBallMaterial;

        [Inject] private InputControls inputControls;
        [Inject] private AchievementManager achievementManager;

        public GameColor Color { get; private set; }
        public Transform ViewModelTransform => viewModel.transform;

        private MeshRenderer meshRenderer;
        private SplineFollower splineFollower;
        private SplineLogic currentSpline;
        private bool isSplineSet;
        private bool alive = true;
        private bool isColorSet;
        private GameObject currentTail;

        private void Awake()
        {
            Assert.IsNotNull(viewModel);
            splineFollower = GetComponent<SplineFollower>();
            splineFollower.onEndReached += OnEndReached;
            inputControls.onPlayerMove += MoveHorizontal;
            MessageBus.Subscribe(this);
            splineFollower.follow = false;
            meshRenderer = viewModel.GetComponent<MeshRenderer>();
        }

        private void OnDestroy()
        {
            if (splineFollower != null)
            {
                splineFollower.onEndReached -= OnEndReached;
            }

            if (inputControls != null)
            {
                inputControls.onPlayerMove -= MoveHorizontal;
            }

            MessageBus.Unsubscribe(this);
        }

        private void MoveHorizontal(float delta)
        {
            if (!alive)
                return;

            (float min, float max) = GetHorizontalMovementLimits();
            Vector3 position = viewModel.transform.localPosition;
            position.x = Mathf.Clamp(position.x + delta, min, max);
            viewModel.transform.localPosition = position;
        }

        private (float min, float max) GetHorizontalMovementLimits()
        {
            float pathWidth = currentSpline.GetPathWidth() - viewModel.transform.lossyScale.x;
            float halfWidth = pathWidth / 2;
            return (-halfWidth, halfWidth);
        }

        private void OnEndReached(double percent)
        {
            splineFollower.follow = false;
            MessageBus.Post(new GenericMessage(GenericMessageType.PlayerFollowPathEnd));
        }

        public void SetSpline(SplineLogic spline, float holeLength)
        {
            splineFollower.followSpeed = spline.FollowSpeed;
            UpdateSpeedParticles(spline);

            if (Mathf.Approximately(spline.FollowSpeed, spline.MaxFollowSpeed))
                achievementManager.UpdateProgress(Achievement.ReachMaxSpeed, 1, true);

            currentSpline = spline;
            splineFollower.spline = currentSpline.GetSpline();
            splineFollower.SetClipRange(0, spline.GetEndPercentage());
            if (!isSplineSet)
            {
                isSplineSet = true;
                return;
            }

            Vector3 jumpPosition = splineFollower.spline.GetPoint(0).position;
            float jumpDuration = holeLength / spline.FollowSpeed;
            LongJump(jumpPosition, jumpDuration);
        }

        private void UpdateSpeedParticles(SplineLogic currentSpline)
        {
            float alpha = RemapFloat(currentSpline.FollowTime,
                minCompletionTime, maxCompletionTime,
                0.5f, 0f);
            SetSpeedParticleAlpha(alpha);
        }

        private void SetSpeedParticleAlpha(float alpha)
        {
            ParticleSystem.MainModule particlesSettings = speedParticles.main;
            ParticleSystem.MinMaxGradient particlesColorGradient = particlesSettings.startColor;
            Color particlesColor = particlesColorGradient.color;
            particlesColor.a = alpha;
            particlesColorGradient.color = particlesColor;
            particlesSettings.startColor = particlesColorGradient;
        }

        private float RemapFloat(float value, float oldMin, float oldMax, float newMin, float newMax)
        {
            return Mathf.Lerp(newMin, newMax, Mathf.InverseLerp(oldMin, oldMax, value));
        }

        public void StartFollowPath(double startPercent = 0)
        {
            splineFollower.follow = true;
            splineFollower.Restart(startPercent);
            MessageBus.Post(new GenericMessage(GenericMessageType.PlayerFollowPathBegin));
        }

        private void LongJump(Vector3 jumpPosition, float jumpDuration)
        {
            transform
                .DOJump(jumpPosition, longJumpHeight, 1, jumpDuration)
                .SetEase(Ease.Linear)
                .OnComplete(delegate
                {
                    StartFollowPath();
                    Hop();
                });
        }

        private void Hop()
        {
            viewModel.transform.DOComplete();
            DOTween.Sequence(viewModel.transform)
                .Append(viewModel.transform.DOLocalMoveY(ballHopHeight, 0.2f).SetRelative())
                .Append(viewModel.transform.DOLocalMoveY(-ballHopHeight, 0.5f).SetRelative().SetEase(Ease.OutBounce));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Obstacle"))
            {
                Obstacle obstacle = other.GetComponentInParent<Obstacle>();
                Assert.IsNotNull(obstacle, "Missing obstacle script!");
                MessageBus.Post(new GenericMessage(GenericMessageType.PlayerObstacleCollision, obstacle));
            }
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.StartGame:
                    StartFollowPath(RoadGenerator.StartOffset / currentSpline.GetPathLength());
                    break;
                case GenericMessageType.PlayerColorChanged:
                    Color = message.Data.Extract<GameColor>();
                    SkinHelper.ApplyBallSkin(meshRenderer, Color, false);
                    if (isColorSet)
                        Hop();
                    isColorSet = true;
                    break;
                case GenericMessageType.GameOver:
                    SetAlive(false);
                    break;
                case GenericMessageType.Respawn:
                    SetAlive(true);
                    float duration = GameManager.INVULNERABILITY_TIME;
                    int blinksCount = 13;
                    float current = 0;
                    DOTween.To(() => current, value =>
                        {
                            current = value;
                            meshRenderer.enabled = current > 0.5f;
                        }, 1, duration / blinksCount).From(0).SetLoops(blinksCount, LoopType.Yoyo);
                    break;
                case GenericMessageType.SkinChanged:
                    GameColor oldColor = Color;
                    Color = GameColors.GetNewColorAfterReskin(Color);
                    if (isColorSet && oldColor != Color)
                    {
                        isColorSet = false;
                        MessageBus.Post(new GenericMessage(GenericMessageType.PlayerColorChanged, Color));
                    }
                    else
                    {
                        SkinHelper.ApplyBallSkin(meshRenderer, Color, false);
                    }
                    if (currentTail != null)
                        Destroy(currentTail);
                    currentTail = Instantiate(SkinHelper.GetTailPrefab(), tailPivot);

                    break;
                case GenericMessageType.BallPickup:
                    float extraScale = 0.2f;
                    float scaleDuration = 0.1f;
                    viewModel.transform.DOScale(Vector3.one * extraScale, scaleDuration)
                        .SetLoops(2, LoopType.Yoyo).SetRelative(true);
                    viewModel.transform.DOLocalMoveY(extraScale / 2, scaleDuration)
                        .SetLoops(2, LoopType.Yoyo).SetRelative(true);
                    var lastMaterial = meshRenderer.material;
                    meshRenderer.material = whiteBallMaterial;
                    UniTask.Delay((int) (scaleDuration * 1000)).GetAwaiter().OnCompleted(() =>
                    {
                        meshRenderer.material = lastMaterial;
                    });
                    break;
            }
        }

        private void SetAlive(bool value)
        {
            splineFollower.follow = value;
            alive = value;
            viewModel.GetComponent<MeshRenderer>().enabled = value;
            shadow.SetActive(value);
            currentTail.SetActive(value);
            speedParticles.gameObject.SetActive(value);
        }
    }
}