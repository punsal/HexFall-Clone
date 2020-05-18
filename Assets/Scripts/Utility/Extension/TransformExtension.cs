using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Extension
{
    public static class TransformExtension
    {
        public static List<T> GetComponentsListInChildren<T>(this Transform parent)
        {
            var components = parent.GetComponentsInChildren<T>().ToList();
            return components;
        }
    }
}