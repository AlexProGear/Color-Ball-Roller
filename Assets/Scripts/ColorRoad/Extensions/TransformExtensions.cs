using UnityEngine;

namespace ColorRoad.Extensions
{
    public static class TransformExtensions
    {
        public static void SetPositionX(this Transform transform, float value)
        {
            Vector3 pos = transform.position;
            pos.x = value;
            transform.position = pos;
        }
        
        public static void SetPositionY(this Transform transform, float value)
        {
            Vector3 pos = transform.position;
            pos.y = value;
            transform.position = pos;
        }
        
        public static void SetPositionZ(this Transform transform, float value)
        {
            Vector3 pos = transform.position;
            pos.z = value;
            transform.position = pos;
        }
        
        public static void SetLocalPositionX(this Transform transform, float value)
        {
            Vector3 pos = transform.localPosition;
            pos.x = value;
            transform.localPosition = pos;
        }
        
        public static void SetLocalPositionY(this Transform transform, float value)
        {
            Vector3 pos = transform.localPosition;
            pos.y = value;
            transform.localPosition = pos;
        }
        
        public static void SetLocalPositionZ(this Transform transform, float value)
        {
            Vector3 pos = transform.localPosition;
            pos.z = value;
            transform.localPosition = pos;
        }
        
        public static void SetLocalScaleX(this Transform transform, float value)
        {
            Vector3 pos = transform.localScale;
            pos.x = value;
            transform.localScale = pos;
        }
        
        public static void SetLocalScaleY(this Transform transform, float value)
        {
            Vector3 pos = transform.localScale;
            pos.y = value;
            transform.localScale = pos;
        }
        
        public static void SetLocalScaleZ(this Transform transform, float value)
        {
            Vector3 pos = transform.localScale;
            pos.z = value;
            transform.localScale = pos;
        }
    }
}