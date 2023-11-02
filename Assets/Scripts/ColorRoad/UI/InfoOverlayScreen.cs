using System.Collections.Generic;
using ColorRoad.Extensions;
using ColorRoad.Gameplay.Obstacles;
using ColorRoad.Systems.Messages;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ColorRoad.UI
{
    public class InfoOverlayScreen : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject overlayElementPrefab;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float messageFarPosition = 1000f;
        [SerializeField] private float messageDuration = 2;
        [SerializeField, Space] private string roadMessage = "Road ";
        [SerializeField, Space] private float ballMessageChance = 0.08f;
        [SerializeField] private string[] ballMessages = { "Awesome!", "Cool!", "You rock!" };
        [SerializeField] private float ballFadeDuration = 1;
        [SerializeField] private float centerFadeDuration = 2;

        private Queue<string> messageQueue = new Queue<string>();
        private bool isShowingMessage;

        private void Awake()
        {
            messageText.text = string.Empty;
            gameObject.SetActive(false);
            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.StartGame:
                    gameObject.SetActive(true);
                    break;
                case GenericMessageType.NewRoad:
                    // ShowTextMessage(roadMessage + message.Data.Extract<int>());
                    ShowCenterTextIndicator(roadMessage + message.Data.Extract<int>(), centerFadeDuration);
                    break;
                case GenericMessageType.BallPickup:
                    if (Random.value < ballMessageChance)
                    {
                        // ShowTextMessage(ballMessages.GetRandom());
                        ShowCenterTextIndicator(ballMessages.GetRandom(), centerFadeDuration);
                    }

                    var ball = message.Data.Extract<Obstacle>();
                    ShowTextIndicator("+" + message.Data.Extract<int>(), GetScreenPosition(ball.viewModel.transform), ballFadeDuration);
                    break;
                case GenericMessageType.CoinPickup:
                    var coin = message.Data.Extract<Obstacle>();
                    ShowTextIndicator("+" + message.Data.Extract<int>(), GetScreenPosition(coin.viewModel.transform), ballFadeDuration);
                    break;
            }
        }

        private Vector2 GetCenterMessagePosition()
        {
            Vector2 centerPivotedPosition = messageText.rectTransform.anchoredPosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 centerOffset = screenSize * messageText.rectTransform.anchorMin;
            return centerPivotedPosition + centerOffset;
        }

        private Vector2 GetScreenPosition(Transform objectTransform)
        {
            Vector3 targetPosition = objectTransform.position + Vector3.up;
            Vector2 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
            return screenPosition;
        }

        private void ShowTextMessage(string text)
        {
            if (isShowingMessage)
            {
                messageQueue.Enqueue(text);
                return;
            }

            isShowingMessage = true;
            messageText.text = text;

            RectTransform target = messageText.rectTransform;
            target.SetAnchoredPositionX(-messageFarPosition);
            DOTween.Sequence()
                .Append(target.DOAnchorPosX(0, messageDuration / 2).SetEase(Ease.OutSine))
                .Append(target.DOAnchorPosX(messageFarPosition, messageDuration / 2).SetEase(Ease.InSine))
                .OnComplete(OnMessageFinished);

            void OnMessageFinished()
            {
                isShowingMessage = false;
                if (messageQueue.Count > 0)
                {
                    ShowTextMessage(messageQueue.Dequeue());
                }
            }
        }

        private void ShowTextIndicator(string text, Vector2 screenPosition, float duration)
        {
            var overlayElement = Instantiate(overlayElementPrefab, transform).GetComponent<InfoOverlayElement>();
            overlayElement.Setup(text, screenPosition);
            overlayElement.Fade(duration);
        }

        private void ShowCenterTextIndicator(string text, float duration)
        {
            var overlayElement = Instantiate(overlayElementPrefab, messageText.rectTransform).GetComponent<InfoOverlayElement>();
            overlayElement.Setup(text, Vector2.zero);
            var overlayElementRect = overlayElement.GetComponent<RectTransform>();
            Vector2 center = new Vector2(0.5f, 0.5f);
            overlayElementRect.pivot = center;
            overlayElementRect.anchorMin = center;
            overlayElementRect.anchorMax = center;
            overlayElementRect.anchoredPosition = Vector2.zero;
            overlayElement.Fade(duration);
        }
    }
}