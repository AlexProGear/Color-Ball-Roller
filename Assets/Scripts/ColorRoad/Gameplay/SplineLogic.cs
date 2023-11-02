using System.Collections.Generic;
using ColorRoad.Systems;
using Dreamteck.Splines;
using UnityEngine;
using Zenject;

namespace ColorRoad.Gameplay
{
    [RequireComponent(typeof(SplineComputer), typeof(PathGenerator))]
    public class SplineLogic : MonoBehaviour
    {
        [SerializeField] private float roadWidth = 4;
        [SerializeField] private float roadSpecialWidth = 7;
        [SerializeField] private bool onlySpecialCheat;

        [Inject] private SplinePointsGenerator splinePointsGenerator;
        [Inject] private ObstacleGenerator obstacleGenerator;
        [Inject] private BuildingGenerator buildingGenerator;

        public readonly List<GameObject> AttachedObjects = new List<GameObject>();

        public float FollowTime { get; private set; }
        public float Length { get; private set; }
        public float FollowSpeed => Length / FollowTime;
        public float MaxFollowSpeed => 13.5f;
        public int RoadNumber { get; private set; }

        private SplineComputer splineComputer;
        private PathGenerator pathGenerator;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            splineComputer = GetComponent<SplineComputer>();
            pathGenerator = GetComponent<PathGenerator>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Generate(Vector3 start, Vector3 direction, float roadLength, int roadNumber)
        {
            AttachedObjects.ForEach(Destroy);
            AttachedObjects.Clear();

            Vector3 end = start + direction.normalized * roadLength;
            splinePointsGenerator.RegenerateSplinePoints(splineComputer, start, end);
            RebuildImmediate();
            pathGenerator.clipTo = splineComputer.Travel(0, roadLength);

            Length = roadLength;
            FollowTime = Mathf.Max(MaxFollowSpeed, 25 - 4 * Mathf.Log(roadNumber));
            RoadNumber = roadNumber;

            bool isSpecial = onlySpecialCheat || roadNumber % 5 == 0;
            SetPathWidth(isSpecial ? roadSpecialWidth : roadWidth);
            obstacleGenerator.Generate(this, isSpecial);
            buildingGenerator.Generate(this);
        }

        private void SetPathWidth(float value)
        {
            pathGenerator.size = value;
        }

        public float GetPathWidth()
        {
            return pathGenerator.size;
        }

        public float GetPathLength()
        {
            return splineComputer.CalculateLength(0, GetEndPercentage());
        }

        public SplineSample GetPointAt(float percentage)
        {
            float distance = Mathf.Lerp((float)pathGenerator.clipFrom, (float)pathGenerator.clipTo, percentage);
            return splineComputer.Evaluate(distance);
        }

        public void RebuildImmediate()
        {
            splineComputer.RebuildImmediate();
            pathGenerator.RebuildImmediate();
            pathGenerator.UpdateCollider();
        }

        public EndPointInfo GetStartPoint()
        {
            Vector3 point1 = splineComputer.GetPoint(0).position;
            Vector3 point2 = splineComputer.GetPoint(1).position;
            return new EndPointInfo()
            {
                position = point1,
                direction = (point2 - point1).normalized
            };
        }

        public EndPointInfo GetEndPoint()
        {
            var pathEndPercent = pathGenerator.clipTo;
            var endPoint = splineComputer.Evaluate(pathEndPercent);
            return new EndPointInfo
            {
                position = endPoint.position,
                direction = endPoint.forward
            };
        }

        public double GetEndPercentage()
        {
            return pathGenerator.clipTo;
        }

        public SplineComputer GetSpline()
        {
            return splineComputer;
        }

        public void ApplyColor(GameColor color)
        {
            SkinHelper.ApplyRoadSkin(meshRenderer, color);
        }
    }

    public struct EndPointInfo
    {
        /// <summary> Point position </summary>
        public Vector3 position;

        /// <summary> Normalized direction vector </summary>
        public Vector3 direction;
    }
}