using System;
using System.Reflection;
using UnityEngine;

namespace Utility.Editor
{
    public abstract class ExtendedInspector<T> : UnityEditor.Editor where T : class
    {
        protected T GenericObject;
        protected Type GenericObjectType;

        private void OnEnable()
        {
            OnEnableAction();
        }

        protected abstract void OnEnableAction();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var methodInfos =
                GenericObjectType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.GetParameters().Length != 0) continue;
                // ReSharper disable once InvertIf
                if (GUILayout.Button(methodInfo.Name))
                    //var genericObjectInstance = Activator.CreateInstance(GenericObjectType);
                    methodInfo.Invoke(GenericObject, null);
            }
        }
    }
}