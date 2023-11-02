using ColorRoad.Extensions;
using ColorRoad.Systems.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class GameScreen : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject pauseOverlay;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button continueButton;
        [SerializeField] private Button homeButton;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text roadNumberText;
        [SerializeField] private string scoreFormattedText = "<color=#9cf346>Score</color> {0}";
        [SerializeField] private string roadFormattedText = "<color=#9cf346>Road</color> {0}";

        private int roadNumber;

        private void Awake()
        {
            pauseButton.onClick.AddListener(OnClickPause);
            continueButton.onClick.AddListener(OnClickContinue);
            homeButton.onClick.AddListener(OnClickHome);
            gameObject.SetActive(false);
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private void OnClickPause()
        {
            pauseOverlay.SetActive(true);
            MessageBus.Post(new GenericMessage(GenericMessageType.PauseGame));
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void OnClickContinue()
        {
            pauseOverlay.SetActive(false);
            MessageBus.Post(new GenericMessage(GenericMessageType.ContinueGame));
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void OnClickHome()
        {
            MessageBus.Post(new GenericMessage(GenericMessageType.ContinueGame));
            MessageBus.Post(new GenericMessage(GenericMessageType.ReturnToMainMenu, false));
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.StartGame:
                    gameObject.SetActive(true);
                    break;
                case GenericMessageType.GameOver:
                    gameObject.SetActive(false);
                    break;
                case GenericMessageType.Respawn:
                    gameObject.SetActive(true);
                    break;
                case GenericMessageType.PlayerFollowPathBegin:
                    roadNumberText.text = string.Format(roadFormattedText, ++roadNumber);
                    break;
                case GenericMessageType.ScoreChanged:
                    scoreText.text = string.Format(scoreFormattedText, message.Data.Extract<int>());
                    break;
            }
        }
    }
}