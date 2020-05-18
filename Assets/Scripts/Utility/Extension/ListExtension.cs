using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility.Custom;

namespace Utility.Extension
{
    public static class ListExtension
    {
        public static void Shuffle<T>(this List<T> list)
        {
            var random = Math.Combination(
                list.Count,
                list.Count);

            var temp = list.ToList();
            list.Clear();

            foreach (var index in random) list.Add(temp[index]);
        }

        public static void ClearGarbage<T>(this List<T> list) where T : Object
        {
            var temp = new List<T>();

            for (var i = 0; i < list.Count; i++)
            {
                if (list[i] == null) continue;
                temp.Add(list[i]);
            }

            list.Clear();
            foreach (var item in temp) list.Add(item);
        }
    }
}