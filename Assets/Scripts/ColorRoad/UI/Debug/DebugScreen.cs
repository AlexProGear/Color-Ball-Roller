using ColorRoad.Systems;
using ColorRoad.Systems.Messages;
using ColorRoad.Systems.Shop;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ColorRoad.UI
{
    public class DebugScreen : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private Button godModeButton;
        [SerializeField] private Button moneyButton;
        [SerializeField] private Button unlockShopButton;
        private bool isGodModeEnabled;

        [Inject] private PlayerAccount playerAccount;
        [Inject] private ShopManager shopManager;

        void Start()
        {
            godModeButton.image.color = Color.red;
            unlockShopButton.image.color = Color.red;
            godModeButton.onClick.AddListener(GodModeClicked);
            moneyButton.onClick.AddListener(AddMoneyClicked);
            unlockShopButton.onClick.AddListener(UnlockShopClicked);
        }

        private void GodModeClicked()
        {
            isGodModeEnabled = !isGodModeEnabled;
            MessageBus.Post(new CheatMessage(Cheat.GodMode, isGodModeEnabled));
            godModeButton.image.color = isGodModeEnabled ? Color.green : Color.red;
        }

        private void AddMoneyClicked()
        {
            playerAccount.Coins.Value += 100;
        }

        private void UnlockShopClicked()
        {
            shopManager.CheatAllItemsUnlocked = !shopManager.CheatAllItemsUnlocked;
            unlockShopButton.image.color = shopManager.CheatAllItemsUnlocked ? Color.green : Color.red;
        }
#else
        private void Awake()
        {
            Destroy(gameObject);
        }
#endif
    }
}