using System.Collections.Generic;
using UnityEngine;

namespace ColorRoad.Systems
{
    public class CachedResources
    {
        private static Dictionary<string, object> loadedResources = new Dictionary<string, object>();
        public static T Get<T>(string path) where T : Object
        {
            if (loadedResources.TryGetValue(path, out object obj))
                return (T)obj;
            T result = Resources.Load<T>(path);
            loadedResources.Add(path, result);
            return result;
        }
    }
}