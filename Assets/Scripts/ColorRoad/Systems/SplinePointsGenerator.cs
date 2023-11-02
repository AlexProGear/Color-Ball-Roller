using System.Collections.Generic;
using Dreamteck.Splines;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColorRoad.Systems
{
    public class SplinePointsGenerator : MonoBehaviour
    {
        [SerializeField, Range(2, 30)] private int pointsCount;
        [SerializeField] private Vector3 randomSpread;
        [SerializeField] private float minPointsDelta;
        [SerializeField] private int startStraightPoints;
        [SerializeField] private int endStraightPoints;
        private float minPointsDeltaSquared;

        private void Awake()
        {
            minPointsDeltaSquared = minPointsDelta * minPointsDelta;
        }

        [Button]
        public void RegenerateSplinePoints(SplineComputer spline, Vector3 start, Vector3 end)
        {
            var points = GeneratePointsArray(start, end);
            spline.SetPoints(points);
        }

        private SplinePoint[] GeneratePointsArray(Vector3 start, Vector3 end)
        {
            List<SplinePoint> resultList = new List<SplinePoint>();
            resultList.Add(new SplinePoint(start));
            Vector3 forward = (end - start).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;
            Vector3 current = start;
            for (int i = 1; i < pointsCount - 1; i++)
            {
                Vector3 stableOffset = (end - current) / (pointsCount - i);
                Vector3 nextOffset = stableOffset;
                bool straightPart = (i <= startStraightPoints || i >= pointsCount - 1 - endStraightPoints); // Начало и конец
                if (straightPart)
                {
                    // Выпрямляем участки в начале и конце сплайна
                    nextOffset.y = (i < pointsCount / 2) ? 0 : end.y - current.y;
                }
                else
                {
                    Vector3 randomOffset = Vector3.Scale(Random.insideUnitSphere, randomSpread);
                    nextOffset += right * randomOffset.x + Vector3.up * randomOffset.y + forward * randomOffset.z;

                    // Исправляем отступ назад
                    if (Vector3.Dot(stableOffset, nextOffset) < 0)
                    {
                        nextOffset = stableOffset - randomOffset;
                    }

                    // Устанавливаем минимальный отступ от предыдущей точки
                    if (nextOffset.sqrMagnitude < minPointsDeltaSquared)
                    {
                        nextOffset = nextOffset.normalized * minPointsDelta;
                    }
                }

                current += nextOffset;
                resultList.Add(new SplinePoint(current));
            }

            resultList.Add(new SplinePoint(end));
            return resultList.ToArray();
        }
    }
}