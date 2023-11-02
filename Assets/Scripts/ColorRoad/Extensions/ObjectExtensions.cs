using System.Collections.Generic;

namespace ColorRoad.Extensions
{
    public static class ObjectExtensions
    {
        public static T Extract<T>(this IEnumerable<object> objects, int index = 0)
        {
            int currentIndex = 0;
            foreach (object current in objects)
            {
                if (current is T castedCurrent)
                {
                    if (currentIndex == index)
                        return castedCurrent;
                    currentIndex++;
                }
            }

            return default;
        }
    }
}