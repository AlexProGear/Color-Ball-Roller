using System;
using ColorRoad.Systems.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private GameObject itemAcquiredFlag;
        [SerializeField] private GameObject priceTag;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private GameObject progressBar;
        [SerializeField] private Image progressImage;
        [SerializeField] private GameObject itemSelected;

        public event Action<ShopItem> OnClick;
        public IPurchasable HeldItem { get; private set; }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(ButtonClicked);
        }

        private void ButtonClicked()
        {
            OnClick?.Invoke(this);
        }

        /// <summary> Basic setup for shop item </summary>
        /// <param name="item"> Associated purchasable item </param>
        /// <param name="acquired"> If true then item is already purchased </param>
        /// <param name="price"> If greater than zero, price tag is used </param>
        /// <param name="progress"> This value is used if item is not acquired and price is below zero </param>
        public void Setup(IPurchasable item, bool acquired, int price = 0, float progress = 0)
        {
            HeldItem = item;
            itemIcon.sprite = item.ShopItemIcon;
            if (acquired)
            {
                itemAcquiredFlag.SetActive(true);
                return;
            }

            if (price > 0)
            {
                priceText.text = price.ToString();
                priceTag.SetActive(true);
                return;
            }

            progressBar.SetActive(true);
            progressImage.fillAmount = progress;
        }

        public void SetSelected(bool selected)
        {
            itemSelected.SetActive(selected);
        }
    }
}