using UnityEngine;

namespace ColorRoad.Extensions
{
    public static class RectTransformExtensions
    {
        public static void SetAnchoredPositionX(this RectTransform transform, float value)
        {
            Vector2 pos = transform.anchoredPosition;
            pos.x = value;
            transform.anchoredPosition = pos;
        }
        
        public static void SetAnchoredPositionY(this RectTransform transform, float value)
        {
            Vector2 pos = transform.anchoredPosition;
            pos.y = value;
            transform.anchoredPosition = pos;
        }
    }
}