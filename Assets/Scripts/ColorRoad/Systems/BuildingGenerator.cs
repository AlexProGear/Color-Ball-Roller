using ColorRoad.Extensions;
using ColorRoad.Gameplay;
using ColorRoad.Systems.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ColorRoad.Systems
{
    public class BuildingGenerator : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private int countPerSpline;
        [SerializeField, Range(0, 300f)] private float horizontalOffset = 150f;
        [SerializeField, Range(0, 300f)] private float horizontalBonusOffset = 20f;
        [SerializeField, Range(0, 300f)] private float forwardBonusOffset = 50f;
        [SerializeField, Range(0, 300f)] private float verticalBonusOffset = 100f;

        private GameColor playerColor;

        private void Awake()
        {
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        public async void Generate(SplineLogic spline)
        {
            var points = spline.GetSpline().rawSamples;
            int stepSize = points.Length / countPerSpline;
            for (int i = 0; i < countPerSpline; i++)
            {
                int index = i * stepSize;
                index = (index < points.Length) ? index : points.Length - 1;
                var point = points[index];
                for (int sign = -1; sign <= 1; sign += 2)
                {
                    var newBuilding = Instantiate(buildingPrefab, spline.transform);
                    float offsetX = horizontalOffset * sign + RandomInRange(horizontalBonusOffset);
                    float offsetY = RandomInRange(verticalBonusOffset);
                    float offsetZ = RandomInRange(forwardBonusOffset);
                    Vector3 forward = point.forward.WithY(0).normalized;
                    Vector3 up = point.up;
                    Vector3 right = point.right.WithY(0).normalized;
                    Vector3 offset = right * offsetX + up * offsetY + forward * offsetZ;
                    newBuilding.transform.position = point.position + offset;
                    newBuilding.GetComponent<Building>().ApplyColor(playerColor);
                    spline.AttachedObjects.Add(newBuilding);
                    await UniTask.NextFrame();
                }
            }
        }

        private static float RandomInRange(float range)
        {
            return Random.Range(-range, range);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerColorChanged:
                    playerColor = message.Data.Extract<GameColor>();
                    break;
            }
        }
    }
}