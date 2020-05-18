using UnityEngine;

namespace Utility.Extension
{
    public static class GameObjectExtension
    {
        public static bool GetComponent<T>(this GameObject gameObject, out T type)
        {
            type = gameObject.GetComponent<T>();
            return type != null;
        }
    }
}