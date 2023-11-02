using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class InfoOverlayElement : MonoBehaviour
    {
        [SerializeField] private TMP_Text textField;
        [SerializeField] private Image image;
        [SerializeField] private float fadeHeight = 100;
        private RectTransform rectTransform;

        private void Init()
        {
            textField.enabled = false;
            image.enabled = false;
            rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            DOTween.Kill(gameObject);
        }

        public void Setup(string text, Vector2 screenPosition)
        {
            Init();
            textField.enabled = true;
            textField.text = text;
            rectTransform.anchoredPosition = screenPosition;
        }

        public void Setup(Sprite sprite, Vector2 screenPosition)
        {
            Init();
            image.enabled = true;
            image.sprite = sprite;
            rectTransform.anchoredPosition = screenPosition;
        }

        public void Fade(float duration)
        {
            DOTween.Sequence()
                .Join(rectTransform.DOAnchorPosY(fadeHeight, duration).SetRelative(true))
                .Join(textField.DOFade(0, duration))
                .Join(image.DOFade(0, duration))
                .OnComplete(() => Destroy(gameObject));
        }
    }
}