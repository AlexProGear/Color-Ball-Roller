using System;
using UnityEngine;

namespace ColorRoad.Systems.Shop
{
    public enum PurchasableType
    {
        None,
        PlayerSkin,
        RoadSkin,
        TailSkin
    }

    public interface IPurchasable
    {
        int PurchasableID { get; }
        PurchasableType PurchasableType { get; }
        Sprite ShopItemIcon { get; }
        PurchasablePrice ShopPrice { get; }
    }

    [Serializable]
    public struct PurchasablePrice
    {
        public int currencyCount;
        public bool useAchievementAsCurrency;
        public Achievement achievement;
    }
}