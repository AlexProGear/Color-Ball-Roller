using System;
using System.Collections.Generic;
using System.Linq;

namespace ColorRoad.Extensions
{
    public static class CollectionExtensions
    {
        /// <summary> Returns random element from collection </summary>
        /// <param name="collection"> Collection </param>
        /// <param name="random"> Optional seeded random number generator (otherwise uses UnityEngine.Random) </param>
        /// <typeparam name="T"> Object type </typeparam>
        /// <returns> Random collection element </returns>
        public static T GetRandom<T>(this IEnumerable<T> collection, Random random = null)
        {
            return collection.OrderBy(e => random?.Next() ?? UnityEngine.Random.value).FirstOrDefault();
        }

        /// <summary> Returns scrambled collection </summary>
        /// <param name="collection"> Collection </param>
        /// <param name="random"> Optional seeded random number generator (otherwise uses UnityEngine.Random) </param>
        /// <typeparam name="T"> Object type </typeparam>
        /// <returns> Scrambled collection </returns>
        public static IEnumerable<T> GetScrambled<T>(this IEnumerable<T> collection, Random random = null)
        {
            return collection.OrderBy(e => random?.Next() ?? UnityEngine.Random.value);
        }
    }
}