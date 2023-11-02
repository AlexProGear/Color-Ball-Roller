using System.Collections.Generic;
using ColorRoad.Systems.Shop;
using UnityEngine;
using UnityEngine.UI;

namespace ColorRoad.UI
{
    public class NewItemsPopupPanel : MonoBehaviour
    {
        [SerializeField] private Image itemDisplay;
        [SerializeField] private Button close;

        public static NewItemsPopupPanel Instance;

        private Queue<IPurchasable> displayQueue = new Queue<IPurchasable>();

        private void Awake()
        {
            Instance = this;
            gameObject.SetActive(false);
            close.onClick.AddListener(ClosePressed);
        }

        public void DisplayItems(IEnumerable<IPurchasable> items)
        {
            foreach (IPurchasable item in items)
            {
                displayQueue.Enqueue(item);
            }

            DisplayNextQueueItem();

            gameObject.SetActive(true);
        }

        private void ClosePressed()
        {
            if (displayQueue.Count > 0)
            {
                DisplayNextQueueItem();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void DisplayNextQueueItem()
        {
            itemDisplay.sprite = displayQueue.Dequeue().ShopItemIcon;
        }
    }
}