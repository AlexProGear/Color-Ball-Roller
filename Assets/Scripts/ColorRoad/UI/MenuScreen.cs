using ColorRoad.Extensions;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ColorRoad.UI
{
    public class MenuScreen : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [BoxGroup("Settings"), SerializeField] private Button settingsButton;
        [BoxGroup("Settings"), SerializeField] private RectTransform settingsPanel;
        [BoxGroup("Settings"), SerializeField, LabelText("Panel position range"), MinMaxSlider(-200, 200, true)]
        private Vector2 settingsPanelPositionRange;
        [BoxGroup("Settings"), SerializeField, LabelText("Panel tween duration")]
        private float settingsPanelTweenDuration;
        [BoxGroup("Settings"), SerializeField] private Button vibrationButton;
        [BoxGroup("Settings"), SerializeField] private Button soundButton;
        [BoxGroup("Settings"), SerializeField] private Sprite[] vibrationIcons;
        [BoxGroup("Settings"), SerializeField] private Sprite[] soundIcons;

        [BoxGroup("Score"), SerializeField] private TMP_Text bestScoreText;
        [BoxGroup("Score"), SerializeField] private TMP_Text bestRoadText;

        [SerializeField] private Button tapToStartPanel;
        [SerializeField] private Button shopButton;

        [Inject] private PlayerAccount playerAccount;
        [Inject] private GameSettings gameSettings;

        private bool settingsPanelOpened;

        private void Awake()
        {
            settingsButton.onClick.AddListener(OnSettingsPressed);
            settingsPanel.anchoredPosition = settingsPanel.anchoredPosition.WithX(settingsPanelPositionRange.x);
            tapToStartPanel.onClick.AddListener(OnTapToStartPressed);
            vibrationButton.onClick.AddListener(OnVibrationToggled);
            soundButton.onClick.AddListener(OnSoundToggled);
            shopButton.onClick.AddListener(OnShopButtonPressed);
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private void Start()
        {
            bestRoadText.text = playerAccount.BestRoad.Value.ToString();
            bestScoreText.text = playerAccount.BestScore.Value.ToString();
            UpdateSettingsButtonIcons();
        }

        private void OnSoundToggled()
        {
            gameSettings.SoundEnabled.Value = !gameSettings.SoundEnabled.Value;
            UpdateSettingsButtonIcons();
        }

        private void OnVibrationToggled()
        {
            gameSettings.VibrationEnabled.Value = !gameSettings.VibrationEnabled.Value;
            UpdateSettingsButtonIcons();
        }

        private void UpdateSettingsButtonIcons()
        {
            soundButton.image.sprite = soundIcons[gameSettings.SoundEnabled.Value ? 1 : 0];
            vibrationButton.image.sprite = vibrationIcons[gameSettings.VibrationEnabled.Value ? 1 : 0];
        }

        private void OnSettingsPressed()
        {
            settingsPanel.DOKill();
            settingsPanelOpened = !settingsPanelOpened;
            float endPosX = settingsPanelOpened ? settingsPanelPositionRange.y : settingsPanelPositionRange.x;
            settingsPanel.DOAnchorPosX(endPosX, settingsPanelTweenDuration);
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void OnTapToStartPressed()
        {
            MessageBus.Post(new GenericMessage(GenericMessageType.StartGame));
            gameObject.SetActive(false);
        }

        private void OnShopButtonPressed()
        {
            MessageBus.Post(new GenericMessage(GenericMessageType.OpenShop));
            gameObject.SetActive(false);
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.OpenMainMenu:
                    gameObject.SetActive(true);
                    break;
            }
        }
    }
}