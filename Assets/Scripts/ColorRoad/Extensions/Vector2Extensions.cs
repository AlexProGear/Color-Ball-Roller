using UnityEngine;

namespace ColorRoad.Extensions
{
    public static class Vector2Extensions
    {
        public static Vector2 WithX(this Vector2 vector, float value)
        {
            vector.x = value;
            return vector;
        }
        
        public static Vector2 WithY(this Vector2 vector, float value)
        {
            vector.y = value;
            return vector;
        }
    }
}