using System.Collections.Generic;
using System.Linq;
using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using ColorRoad.Systems.Shop;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ColorRoad.UI
{
    public class ShopScreen : MonoBehaviour, IMessageReceiver<GenericMessage>
    {
        [SerializeField] private GameObject shopItemPrefab;
        [SerializeField] private Button backButton;
        [SerializeField] private ShopTab[] shopTabs;
        [SerializeField] private PurchasableType[] shopItemTypes;
        [SerializeField] private Transform shopContentParent;
        [SerializeField, BoxGroup("Buy")] private GameObject buyItemPanel;
        [SerializeField, BoxGroup("Buy")] private Image buyItemIcon;
        [SerializeField, BoxGroup("Buy")] private Button buyItemButton;
        [SerializeField, BoxGroup("Buy")] private TMP_Text buyItemButtonText;
        [SerializeField, BoxGroup("Buy")] private Sprite availableButtonImage;
        [SerializeField, BoxGroup("Buy")] private Sprite unavailableButtonImage;
        [SerializeField, BoxGroup("Unlock")] private GameObject unlockItemPanel;
        [SerializeField, BoxGroup("Unlock")] private Image unlockItemIcon;
        [SerializeField, BoxGroup("Unlock")] private Image unlockItemProgress;
        [SerializeField, BoxGroup("Unlock")] private TMP_Text unlockItemText;

        [Inject] private ShopManager shopManager;
        [Inject] private AchievementManager achievementManager;

        private readonly List<GameObject> currentItems = new List<GameObject>();
        private IPurchasable currentItem;
        private IPurchasable[] itemsToDisplay;

        private void Awake()
        {
            gameObject.SetActive(false);
            backButton.onClick.AddListener(OnBackButtonPressed);
            for (int i = 0; i < shopTabs.Length; i++)
            {
                ShopTab shopTab = shopTabs[i];
                int tabIndex = i;
                shopTab.Clicked += () => OnShopTabClicked(tabIndex);
            }

            buyItemButton.onClick.AddListener(OnBuyItemPressed);

            MessageBus.Subscribe(this);
        }

        private void OnDestroy()
        {
            MessageBus.Unsubscribe(this);
        }

        private void OnEnable()
        {
            OnShopTabClicked(0);
        }

        private void OnBuyItemPressed()
        {
            if (!shopManager.TryPurchaseItem(currentItem))
                return;
            buyItemPanel.SetActive(false);
            shopManager.EquipItem(currentItem);
            UpdateDisplayItems();
        }

        private void OnBackButtonPressed()
        {
            MessageBus.Post(new GenericMessage(GenericMessageType.OpenMainMenu));
            gameObject.SetActive(false);
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void OnShopTabClicked(int tabIndex)
        {
            shopTabs.ForEach(tab => tab.SetEnabled(false));
            shopTabs[tabIndex].SetEnabled(true);
            PurchasableType itemsType = shopItemTypes[tabIndex];
            itemsToDisplay = shopManager.PurchasableItems.Where(item => item.PurchasableType == itemsType).ToArray();
            UpdateDisplayItems();
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void UpdateDisplayItems()
        {
            currentItems.ForEach(Destroy);
            currentItems.Clear();

            if (itemsToDisplay.Length == 0)
                Debug.LogWarning("Shop: No items to display");

            foreach (var item in itemsToDisplay)
            {
                var newItem = Instantiate(shopItemPrefab, shopContentParent).GetComponent<ShopItem>();
                bool acquired = shopManager.IsPurchasedOrUnlocked(item);
                int price = item.ShopPrice.useAchievementAsCurrency ? 0 : item.ShopPrice.currencyCount;
                float progress = achievementManager.GetProgress(item.ShopPrice.achievement);
                progress /= item.ShopPrice.currencyCount;
                newItem.Setup(item, acquired, price, progress);
                newItem.OnClick += OnItemClick;
                newItem.SetSelected(shopManager.IsEquipped(item));
                currentItems.Add(newItem.gameObject);
            }

            UpdateSelection();
        }

        private void UpdateSelection()
        {
            foreach (var item in currentItems)
            {
                var shopItem = item.GetComponent<ShopItem>();
                shopItem.SetSelected(shopManager.IsEquipped(shopItem.HeldItem));
            }
        }

        private void OnItemClick(ShopItem item)
        {
            currentItem = item.HeldItem;
            if (shopManager.IsPurchasedOrUnlocked(currentItem))
            {
                shopManager.EquipItem(currentItem);
                UpdateSelection();
                return;
            }

            if (currentItem.ShopPrice.useAchievementAsCurrency)
                ShowUnlockPanel(currentItem);
            else
                ShowBuyPanel(currentItem);
            MessageBus.Post(new AudioMessage(AudioMessageType.Click));
        }

        private void ShowBuyPanel(IPurchasable item)
        {
            buyItemIcon.sprite = item.ShopItemIcon;
            buyItemButtonText.text = item.ShopPrice.currencyCount.ToString();
            buyItemPanel.SetActive(true);
            Sprite buttonSprite = shopManager.CanPurchaseItem(item) ? availableButtonImage : unavailableButtonImage;
            buyItemButton.image.sprite = buttonSprite;
        }

        private void ShowUnlockPanel(IPurchasable item)
        {
            unlockItemIcon.sprite = item.ShopItemIcon;
            Achievement achievement = item.ShopPrice.achievement;
            int requiredProgress = item.ShopPrice.currencyCount;
            float progress = (float)achievementManager.GetProgress(achievement) / requiredProgress;
            unlockItemProgress.fillAmount = progress;
            string progressText = string.Format(achievementManager.GetAchievementText(achievement), requiredProgress);
            unlockItemText.text = progressText;
            unlockItemPanel.SetActive(true);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.OpenShop:
                    gameObject.SetActive(true);
                    break;
            }
        }
    }
}