using ColorRoad.Extensions;
using ColorRoad.Gameplay;
using ColorRoad.Systems.Messages;
using ModestTree;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems
{
    public class RoadGenerator : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject roadPrefab;
        [SerializeField] private float roadLength = 1000;
        [SerializeField] private float holeLength = 70;

        [Inject] private GameInstaller zenject;
        [Inject] private PlayerLogic player;

        public const int StartOffset = 10;

        private SplineLogic currentRoad;
        private SplineLogic nextRoad;
        private int roadNumber;
        private GameColor currentColor;

        private void Awake()
        {
            Assert.IsNotNull(roadPrefab);
            currentRoad = zenject.InstantiateAndInject(roadPrefab).GetComponent<SplineLogic>();
            nextRoad = zenject.InstantiateAndInject(roadPrefab).GetComponent<SplineLogic>();
            MessageBus.Subscribe(this);
        }

        public void Initialize()
        {
            currentRoad.Generate(Vector3.back * StartOffset, Vector3.forward, roadLength, ++roadNumber);
            currentRoad.RebuildImmediate();
            SendCurrentRoadToPlayer();
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private void SendCurrentRoadToPlayer()
        {
            player.SetSpline(currentRoad, holeLength);
        }

        private void PrepareNextRoad()
        {
            EndPointInfo currentEndPoint = currentRoad.GetEndPoint();
            Vector3 nextStart = currentEndPoint.position + currentEndPoint.direction * holeLength;
            nextRoad.Generate(nextStart, currentEndPoint.direction, roadLength, ++roadNumber);
        }

        private void SwapRoads()
        {
            (currentRoad, nextRoad) = (nextRoad, currentRoad);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.PlayerFollowPathBegin:
                    PrepareNextRoad();
                    break;
                case GenericMessageType.PlayerFollowPathEnd:
                    SwapRoads();
                    SendCurrentRoadToPlayer();
                    break;
                case GenericMessageType.PlayerColorChanged:
                    currentColor = message.Data.Extract<GameColor>();
                    goto case GenericMessageType.SkinChanged;
                case GenericMessageType.SkinChanged:
                    currentColor = GameColors.GetNewColorAfterReskin(currentColor);
                    currentRoad.ApplyColor(currentColor);
                    nextRoad.ApplyColor(currentColor);
                    break;
            }
        }
    }
}