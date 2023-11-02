using UnityEngine;

namespace ColorRoad.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 WithX(this Vector3 vector, float value)
        {
            vector.x = value;
            return vector;
        }
        
        public static Vector3 WithY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }
        
        public static Vector3 WithZ(this Vector3 vector, float value)
        {
            vector.z = value;
            return vector;
        }
    }
}