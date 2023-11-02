using ColorRoad.Systems;
using ColorRoad.Systems.Shop;
using ModestTree;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColorRoad.ScriptableObjects
{
    public abstract class SkinBase : ScriptableObject, IPurchasable
    {
        [field: SerializeField] public Material material { get; private set; }
        [field: SerializeField] public Texture2D[] textures { get; private set; }

        public GameColor[] colors;

        [field: SerializeField, BoxGroup("Shop Item")]
        public int PurchasableID { get; private set; }

        [field: SerializeField, BoxGroup("Shop Item")]
        public PurchasableType PurchasableType { get; private set; }
        
        [field: SerializeField, BoxGroup("Shop Item")]
        public Sprite ShopItemIcon { get; private set; }

        [field: SerializeField, BoxGroup("Shop Item")]
        public PurchasablePrice ShopPrice { get; private set; }

        public Texture2D GetTextureByColor(GameColor color)
        {
            int index = colors.IndexOf(color);
            if (index < 0)
                return null;
            return textures[index];
        }
    }
}