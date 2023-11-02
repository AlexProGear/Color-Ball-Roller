using ColorRoad.Systems.Shop;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColorRoad.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Trail Skin", menuName = "Skins/Trail Skin", order = 3)]
    public class TailSkin : ScriptableObject, IPurchasable
    {
        public GameObject effectPrefab;
        [field: SerializeField, BoxGroup("Shop Item")] public int PurchasableID { get; private set; }
        [field: SerializeField, BoxGroup("Shop Item")] public PurchasableType PurchasableType { get; private set; }
        [field: SerializeField, BoxGroup("Shop Item")] public Sprite ShopItemIcon { get; private set; }
        [field: SerializeField, BoxGroup("Shop Item")] public PurchasablePrice ShopPrice { get; private set; }
    }
}