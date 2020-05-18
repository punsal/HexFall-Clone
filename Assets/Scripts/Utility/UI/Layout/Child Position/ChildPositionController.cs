using System;
using UnityEngine;

/*
 * This script is not ready to use!
 * TODO: Finish this editor behaviour
 */

namespace Utility.UI.Layout.Child_Position
{
    public enum LayoutDirection
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public enum SnapType
    {
        Width,
        Height
    }

    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class ChildPositionController : MonoBehaviour
    {
        [SerializeField] private LayoutDirection direction = LayoutDirection.Left;
        private RectTransform rectTransform;
#pragma warning disable 414
        [SerializeField] private SnapType snapType = SnapType.Width;
#pragma warning restore 414

        private void OnEnable()
        {
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        }

#if UNITY_EDITOR

        private void Update()
        {
            if (Application.isPlaying) return;
            var sizeDelta = rectTransform.sizeDelta;

            switch (direction)
            {
                case LayoutDirection.Left:
                    break;
                case LayoutDirection.Top:
                    break;
                case LayoutDirection.Right:
                    break;
                case LayoutDirection.Bottom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

#endif
    }
}