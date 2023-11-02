using ColorRoad.Extensions;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace ColorRoad.UI
{
    public class GameOverScreen : MonoBehaviour, IMessageReceiver<GenericMessage>, IPointerDownHandler,
        IPointerUpHandler
    {
        [SerializeField] private int fadeDuration = 1;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text roadText;
        [SerializeField] private string scoreFormattedText = "<color=#9cf346>Score</color> {0}";
        [SerializeField] private string roadFormattedText = "<color=#9cf346>Road</color> {0}";
        [SerializeField] private GameObject newScore;
        [SerializeField] private GameObject newRoad;

        [Inject] private AchievementManager achievementManager;

        private CanvasGroup canvasGroup;
        private int roadNumber;

        private void Awake()
        {
            gameObject.SetActive(false);
            MessageBus.Subscribe(this);
            newScore.SetActive(false);
            newRoad.SetActive(false);
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.GameOver:
                    gameObject.SetActive(true);
                    canvasGroup.DOFade(1, fadeDuration).From(0)
                        .OnComplete(() =>
                        {
                            canvasGroup.interactable = true;
                            canvasGroup.blocksRaycasts = true;
                        });
                    int score = message.Data.Extract<int>(0);
                    roadNumber = message.Data.Extract<int>(1);
                    scoreText.text = string.Format(scoreFormattedText, score);
                    roadText.text = string.Format(roadFormattedText, roadNumber);

                    break;
                case GenericMessageType.Respawn:
                    gameObject.SetActive(false);
                    break;
                case GenericMessageType.NewBestScore:
                    newScore.SetActive(true);
                    break;
                case GenericMessageType.NewBestRoad:
                    newRoad.SetActive(true);
                    break;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Required for OnPointerUp to work
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            MessageBus.Post(new GenericMessage(GenericMessageType.ReturnToMainMenu, true));
        }
    }
}