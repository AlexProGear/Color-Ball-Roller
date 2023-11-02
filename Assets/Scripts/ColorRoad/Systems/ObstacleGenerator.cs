using System;
using System.Collections.Generic;
using System.Linq;
using ColorRoad.Extensions;
using ColorRoad.Gameplay;
using ColorRoad.Gameplay.Obstacles;
using ColorRoad.Systems.Messages;
using ModestTree;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ColorRoad.Systems
{
    public class ObstacleGenerator : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private float startOffset = 0.09f;
        [SerializeField] private float middleBaseOffset = 0.1f;
        [SerializeField] private float middleExtraOffset = 0.025f;
        [SerializeField] private float endBaseOffset = 0.025f;
        [SerializeField] private float endExtraOffset = 0f;
        
        [InfoBox("@\"Offset = \" + a + \" * \" + b + \" ^ (\" + c + \" * speed)\"")]
        [SerializeField, BoxGroup] private float a = 0.009f;
        [SerializeField, BoxGroup] private float b = 2.718f;
        [SerializeField, BoxGroup] private float c = 0.01f;

        private const string CollectionWarningMessage = "Prefabs must have Obstacle component attached!";

        [InfoBox(
            "\"Priority\" should be used in conjunction with \"Max Count\", otherwise lower priority items will never be spawned!",
            InfoMessageType.Warning)]
        [SerializeField, TableList, ValidateInput("@ValidateCollection(0)", CollectionWarningMessage)]
        private ObstacleInfo[] middleObstacles;

        [SerializeField, TableList, ValidateInput("@ValidateCollection(1)", CollectionWarningMessage)]
        private ObstacleInfo[] endObstacles;

        [Serializable]
        private class ObstacleInfo
        {
            public GameObject prefab;
            public int maxCount = -1;
            public int minObstaclesBetween;
            public int priority;
        }

        [Inject] private GameInstaller _installer;

        private readonly Dictionary<ObstacleInfo, int> obstaclesBetweenCounter = new Dictionary<ObstacleInfo, int>();
        private readonly Dictionary<ObstacleInfo, int> obstaclesMaxCounter = new Dictionary<ObstacleInfo, int>();
        private GameColor lastSpecialColor;
        private bool isSetLastSpecialColor;

        // ReSharper disable once UnusedMember.Local
        private bool ValidateCollection(int index)
        {
            return index switch
            {
                0 => middleObstacles.All(info => info.prefab.GetComponent<Obstacle>() != null),
                1 => endObstacles.All(info => info.prefab.GetComponent<Obstacle>() != null),
                _ => false
            };
        }

        private void Awake()
        {
            middleObstacles.ForEach(info =>
            {
                obstaclesMaxCounter.Add(info, 0);
                obstaclesBetweenCounter.Add(info, 0);
            });
            endObstacles.ForEach(info =>
            {
                obstaclesMaxCounter.Add(info, 0);
                obstaclesBetweenCounter.Add(info, 0);
            });
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        public void Generate(SplineLogic spline, bool isSpecial)
        {
            Assert.IsEqual(isSetLastSpecialColor, true);
            float endPosition = FillEndObstacles(spline, isSpecial);
            FillMiddleObstacles(spline, isSpecial, endPosition);
        }

        private float FillMiddleObstacles(SplineLogic spline, bool isSpecial, float endPosition)
        {
            float start = startOffset;
            return FillRange(spline, isSpecial, start, endPosition, middleObstacles, middleBaseOffset, middleExtraOffset);
        }

        private float FillEndObstacles(SplineLogic spline, bool isSpecial)
        {
            const float start = 1;
            const float end = 0;
            return FillRange(spline, isSpecial, start, end, endObstacles, endBaseOffset, endExtraOffset);
        }

        /// <summary> Fills spline region with obstacles </summary>
        /// <returns> Last obstacle position </returns>
        private float FillRange(SplineLogic spline, bool isSpecial, float start, float end,
            ObstacleInfo[] obstacleOptions, float baseOffset, float randomOffset)
        {
            if (obstacleOptions == null || obstacleOptions.Length == 0)
            {
                Debug.LogWarning("Obstacle collection doesn't have any elements!", this);
                return start;
            }

            obstacleOptions.ForEach(info => obstaclesMaxCounter[info] = 0);

            List<ObstacleInfo> GetAvailableItems()
            {
                var available = obstacleOptions.Where(info =>
                    (info.maxCount < 0 || obstaclesMaxCounter[info] < info.maxCount) &&
                    (info.minObstaclesBetween == 0 || obstaclesBetweenCounter[info] == 0)).ToList();
                if (available.IsEmpty())
                    return available;
                int maxPriority = available.Max(info => info.priority);
                return available.Where(info => info.priority == maxPriority).ToList();
            }

            void UpdateCounters(ObstacleInfo currentObstacle)
            {
                var keys = obstaclesBetweenCounter.Keys.ToList();
                foreach (var key in keys)
                {
                    if (obstaclesBetweenCounter[key] > 0)
                        obstaclesBetweenCounter[key]--;
                }

                obstaclesBetweenCounter[currentObstacle] = currentObstacle.minObstaclesBetween;
                if (currentObstacle.maxCount >= 0)
                    obstaclesMaxCounter[currentObstacle]++;
            }

            float direction = Mathf.Sign(end - start);
            float position;

            void AddOffset(float value)
            {
                position += value * direction;
            }

            bool IsWithinBounds()
            {
                return direction > 0 ? position < end : position > end;
            }

            for (position = start; IsWithinBounds(); AddOffset(baseOffset + Random.Range(0f, randomOffset)))
            {
                List<ObstacleInfo> availableItems = GetAvailableItems();
                if (availableItems.Count == 0)
                {
                    return position;
                }

                ObstacleInfo theChosenOne = availableItems.GetRandom();

                // Group spawn parameters
                var obstacle = theChosenOne.prefab.GetComponent<Obstacle>();
                int groupSize = Random.Range(obstacle.RandomMinCount, obstacle.RandomMaxCount + 1);

                float GetRandomInnerGroupOffset()
                {
                    // return Random.Range(obstacle.RandomInnerMinOffset, obstacle.RandomInnerMaxOffset);
                    return a * Mathf.Pow(b, c * spline.FollowSpeed);
                }

                int groupRandomSeed = Random.Range(int.MinValue, int.MaxValue);

                // Spawning group
                for (int i = 0; i < groupSize; i++)
                {
                    var spawnPoint = spline.GetPointAt(position);
                    GameObject newObstacle = _installer.InstantiateAndInject(theChosenOne.prefab);
                    newObstacle.transform.SetParent(spline.transform);
                    spline.AttachedObjects.Add(newObstacle);
                    newObstacle.transform.position = spawnPoint.position;
                    newObstacle.transform.rotation = spawnPoint.rotation;

                    Obstacle obstacleComponent = newObstacle.GetComponent<Obstacle>();
                    obstacleComponent.Initialize(spline, isSpecial, ref lastSpecialColor, groupRandomSeed, i, groupSize);

                    if (i < groupSize - 1)
                        AddOffset(GetRandomInnerGroupOffset());
                    if (!IsWithinBounds())
                        break;
                }

                UpdateCounters(theChosenOne);
            }

            return end;
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerColorChanged:
                    if (!isSetLastSpecialColor)
                    {
                        isSetLastSpecialColor = true;
                        lastSpecialColor = message.Data.Extract<GameColor>();
                    }

                    break;
            }
        }
    }
}