using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class ShopTab : MonoBehaviour
    {
        [SerializeField] private Image tabImage;
        [SerializeField] private TMP_Text tabText;
        [SerializeField] private Color tabOffColor;
        [SerializeField] private Color tabOnColor;
        [SerializeField] private Sprite tabOffSprite;
        [SerializeField] private Sprite tabOnSprite;

        public event Action Clicked;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => Clicked?.Invoke());
        }

        public void SetText(string text)
        {
            tabText.text = text;
        }

        public void SetEnabled(bool value)
        {
            tabImage.sprite = value ? tabOnSprite : tabOffSprite;
            tabText.color = value ? tabOnColor : tabOffColor;
        }
    }
}