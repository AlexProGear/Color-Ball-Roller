using System;
using System.Collections.Generic;
using System.Linq;
using ColorRoad.HelperClasses;
using ColorRoad.ScriptableObjects;
using ColorRoad.Systems.Messages;
using ColorRoad.UI;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace ColorRoad.Systems.Shop
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShopManager : IInitializable, IDisposable, IMessageReceiver<GenericMessage>
    {
        [Inject] private AchievementManager achievementManager;
        [Inject] private PlayerAccount playerAccount;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public bool CheatAllItemsUnlocked { get; set; }
#endif
        public IPurchasable[] PurchasableItems { get; private set; }

        private readonly HashSet<int> purchasedItemIDs = new HashSet<int>();
        private readonly HashSet<int> equippedItemIDs = new HashSet<int>();

        private IPurchasable[] lastAvailableItems;

        private static HashSet<IPurchasable> newItems = new HashSet<IPurchasable>();

        public void Initialize()
        {
            List<IPurchasable> skinItems = new List<IPurchasable>();
            skinItems.AddRange(Resources.LoadAll<BallSkin>("Ball Skins"));
            skinItems.AddRange(Resources.LoadAll<RoadSkin>("Road Skins"));
            skinItems.AddRange(Resources.LoadAll<TailSkin>("Tail Skins"));
            skinItems.RemoveAll(item => item.PurchasableType == PurchasableType.None);
#if UNITY_EDITOR
            CheckDuplicates(skinItems);
#endif
            PurchasableItems = skinItems.ToArray();
            LoadItems();
            EquipNewItems();
            MessageBus.Subscribe(this);
        }

        public void Dispose()
        {
            SaveItems();
            MessageBus.Unsubscribe(this);
        }

        private void LoadItems()
        {
            LoadPurchases();
            LoadEquipment();
            UpdateEquippedSkins();
        }

        private void EquipNewItems()
        {
            if (newItems.Count <= 0)
                return;

            NewItemsPopupPanel.Instance.DisplayItems(newItems);
            newItems.GroupBy(item => item.PurchasableType)
                .ForEach(group => EquipItem(@group.Last()));

            newItems.Clear();
        }

        private void LoadPurchases()
        {
            string purchasedIDsJson = PlayerPrefs.GetString("purchased_ids");
            if (purchasedIDsJson.IsNullOrWhitespace())
                return;
            int[] purchasedIDs = JsonArrayHelper.FromJson<int>(purchasedIDsJson);
            if (purchasedIDs.IsNullOrEmpty())
                return;
            foreach (int id in purchasedIDs)
            {
                purchasedItemIDs.Add(id);
            }
        }

        private void LoadEquipment()
        {
            string equippedIDsJson = PlayerPrefs.GetString("equipped_ids");
            int[] equippedIDs;
            if (!equippedIDsJson.IsNullOrWhitespace())
            {
                equippedIDs = JsonArrayHelper.FromJson<int>(equippedIDsJson);
            }
            else
            {
                equippedIDs = PurchasableItems.Where(IsPurchasedOrUnlocked)
                    .GroupBy(item => item.PurchasableType)
                    .Select(group => group.First())
                    .Select(item => item.PurchasableID).ToArray();
            }

            foreach (int id in equippedIDs)
            {
                equippedItemIDs.Add(id);
            }
        }

        private void SaveItems()
        {
            if (purchasedItemIDs.Count > 0)
            {
                string purchasedIDsJson = JsonArrayHelper.ToJson(purchasedItemIDs.ToArray());
                PlayerPrefs.SetString("purchased_ids", purchasedIDsJson);
            }

            if (equippedItemIDs.Count > 0)
            {
                int[] equipmentArray = equippedItemIDs.ToArray();
                string equippedIDsJson = JsonArrayHelper.ToJson(equipmentArray);
                PlayerPrefs.SetString("equipped_ids", equippedIDsJson);
            }
        }

        public bool IsPurchasedOrUnlocked(IPurchasable item)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (CheatAllItemsUnlocked)
                return true;
#endif
            bool achieved = false;
            if (item.ShopPrice.useAchievementAsCurrency)
                achieved = achievementManager.GetProgress(item.ShopPrice.achievement) >= item.ShopPrice.currencyCount;
            return purchasedItemIDs.Contains(item.PurchasableID) || achieved;
        }

        public bool IsEquipped(IPurchasable item)
        {
            return equippedItemIDs.Contains(item.PurchasableID);
        }

        public bool TryPurchaseItem(IPurchasable item)
        {
            if (!CanPurchaseItem(item))
                return false;
            playerAccount.Coins.Value -= item.ShopPrice.currencyCount;
            purchasedItemIDs.Add(item.PurchasableID);
            return true;
        }

        public bool CanPurchaseItem(IPurchasable item)
        {
            return playerAccount.Coins.Value >= item.ShopPrice.currencyCount;
        }

        private IPurchasable[] GetAvailableItems()
        {
            return PurchasableItems.Where(item => IsPurchasedOrUnlocked(item)).ToArray();
        }

        public void EquipItem(IPurchasable item)
        {
            var targetToReplace = PurchasableItems.SingleOrDefault(skin =>
                equippedItemIDs.Contains(skin.PurchasableID) && skin.PurchasableType == item.PurchasableType);
            if (targetToReplace != null)
                equippedItemIDs.Remove(targetToReplace.PurchasableID);
            equippedItemIDs.Add(item.PurchasableID);
            UpdateEquippedSkins();
        }

        private void UpdateEquippedSkins()
        {
            var equippedSkins = PurchasableItems.Where(skin => equippedItemIDs.Contains(skin.PurchasableID)).ToArray();
            BallSkin ballSkin = (BallSkin)equippedSkins.Single(skin => skin is BallSkin);
            RoadSkin roadSkin = (RoadSkin)equippedSkins.Single(skin => skin is RoadSkin);
            TailSkin tailSkin = (TailSkin)equippedSkins.Single(skin => skin is TailSkin);
            SkinHelper.UpdateSkins(ballSkin, roadSkin, tailSkin);
        }

        public void OnMessageReceived(GenericMessage message)
        {
            switch (message.MessageType)
            {
                case GenericMessageType.StartGame:
                    lastAvailableItems = GetAvailableItems();
                    break;
                case GenericMessageType.GameOver:
                    newItems.AddRange(GetAvailableItems().Except(lastAvailableItems).ToArray());
                    break;
            }
        }

#if UNITY_EDITOR
        private static void CheckDuplicates(List<IPurchasable> skinItems)
        {
            var duplicateItems = skinItems.GroupBy(item => item.PurchasableID)
                .Where(g => g.Count() > 1);
            foreach (var group in duplicateItems)
            {
                Debug.LogError($"Duplicate shop items found for ID {group.Key}");
            }
        }
#endif
    }
}