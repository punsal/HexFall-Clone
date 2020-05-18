using UnityEngine;

namespace Utility.Extension
{
    public static class ComponentExtension
    {
        /// <summary>
        ///     Checks component if component exists, outs component; otherwise returns false.
        /// </summary>
        /// <param name="component">A component type besides GameObject</param>
        /// <param name="type">Component object name</param>
        /// <typeparam name="T">Type of requested component</typeparam>
        /// <returns></returns>
        public static bool GetComponent<T>(this Component component, out T type)
        {
            type = component.GetComponent<T>();
            return type != null;
        }

        /// <summary>
        ///     Try to get component. If it does not exist adds new instance.
        /// </summary>
        /// <param name="component">A component type, not GameObject</param>
        /// <typeparam name="T">Type of requested component</typeparam>
        /// <returns>Either existing component or adds new component and returns it</returns>
        public static T TryComponent<T>(this Component component) where T : Component
        {
            if (component.GetComponent<T>(out var requested))
            {
                return requested;
            }

            var result = component.gameObject.AddComponent<T>();
            return result;
        }
    }
}