using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiards
{

    public static class CollectionExtension
    {
        public static T GetRandom<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }
        public static T GetRandom<T>(this List<T> list, Predicate<T> pred)
        {
            return list.Where(new Func<T, bool>(pred)).ToList()[UnityEngine.Random.Range(0, list.Count)];
        }
    }
}

