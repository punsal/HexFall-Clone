using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utility.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasHelper : MonoBehaviour
    {
        public static UnityEvent OnOrientationChange = new UnityEvent();
        public static UnityEvent OnResolutionChange = new UnityEvent();

        private static readonly List<CanvasHelper> Helpers = new List<CanvasHelper>();

        private static bool screenChangeVarsInitialized;
        private static ScreenOrientation lastOrientation = ScreenOrientation.Portrait;
        private static Vector2 lastResolution = Vector2.zero;
        private static Rect lastSafeArea = Rect.zero;

        private Canvas canvas;
        private RectTransform rectTransform;

#pragma warning disable 649
        [SerializeField] private RectTransform safeAreaTransform;
#pragma warning restore 649
        public static bool IsLandscape { get; private set; }

        private void Awake()
        {
            if (!Helpers.Contains(this))
                Helpers.Add(this);

            canvas = GetComponent<Canvas>();
            rectTransform = GetComponent<RectTransform>();

            //safeAreaTransform = transform.GetChild(0) as RectTransform;

            if (screenChangeVarsInitialized) return;
            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;
            lastSafeArea = Screen.safeArea;

            screenChangeVarsInitialized = true;
        }

        private void Start()
        {
            ApplySafeArea();
        }

        private void Update()
        {
            if (Helpers[0] != this)
                return;

            if (Application.isMobilePlatform)
            {
                if (Screen.orientation != lastOrientation)
                    OrientationChanged();

                if (Screen.safeArea != lastSafeArea)
                    SafeAreaChanged();
            }
            else
            {
                //resolution of mobile devices should stay the same always, right?
                // so this check should only happen everywhere else
                if (Math.Abs(Screen.width - lastResolution.x) > 0f || Math.Abs(Screen.height - lastResolution.y) > 0f)
                    ResolutionChanged();
            }
        }

        private void ApplySafeArea()
        {
            if (safeAreaTransform == null)
                return;

            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            var pixelRect = canvas.pixelRect;
            anchorMin.x /= pixelRect.width;
            anchorMin.y /= pixelRect.height;
            anchorMax.x /= pixelRect.width;
            anchorMax.y /= pixelRect.height;

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;

            // Debug.Log(
            // "ApplySafeArea:" +
            // "\n Screen.orientation: " + Screen.orientation +
            // #if UNITY_IOS
            // "\n Device.generation: " + UnityEngine.iOS.Device.generation.ToString() +
            // #endif
            // "\n Screen.safeArea.position: " + Screen.safeArea.position.ToString() +
            // "\n Screen.safeArea.size: " + Screen.safeArea.size.ToString() +
            // "\n Screen.width / height: (" + Screen.width.ToString() + ", " + Screen.height.ToString() + ")" +
            // "\n canvas.pixelRect.size: " + canvas.pixelRect.size.ToString() +
            // "\n anchorMin: " + anchorMin.ToString() +
            // "\n anchorMax: " + anchorMax.ToString());

            if (Screen.height / Screen.width > 1.5f)
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0f;
            else
                GetComponent<CanvasScaler>().matchWidthOrHeight = 1f;
        }

        private void OnDestroy()
        {
            if (Helpers != null && Helpers.Contains(this))
                Helpers.Remove(this);
        }

        private static void OrientationChanged()
        {
            //Debug.Log("Orientation changed from " + lastOrientation + " to " + Screen.orientation + " at " + Time.time);

            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            IsLandscape = lastOrientation == ScreenOrientation.LandscapeLeft ||
                          lastOrientation == ScreenOrientation.LandscapeRight ||
                          lastOrientation == ScreenOrientation.Landscape;
            OnOrientationChange.Invoke();
        }

        private static void ResolutionChanged()
        {
            if (Math.Abs(lastResolution.x - Screen.width) < 0.1f && Math.Abs(lastResolution.y - Screen.height) < 0.1f)
                return;

            //Debug.Log("Resolution changed from " + lastResolution + " to (" + Screen.width + ", " + Screen.height + ") at " + Time.time);

            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            IsLandscape = Screen.width > Screen.height;
            OnResolutionChange.Invoke();
        }

        private static void SafeAreaChanged()
        {
            if (lastSafeArea == Screen.safeArea)
                return;

            //Debug.Log("Safe Area changed from " + lastSafeArea + " to " + Screen.safeArea.size + " at " + Time.time);

            lastSafeArea = Screen.safeArea;

            foreach (var helper in Helpers) helper.ApplySafeArea();
        }

        public static Vector2 GetCanvasSize()
        {
            return Helpers[0].rectTransform.sizeDelta;
        }

        public static Vector2 GetSafeAreaSize()
        {
            foreach (var helper in Helpers.Where(helper => helper.safeAreaTransform != null))
                return helper.safeAreaTransform.sizeDelta;

            return GetCanvasSize();
        }
    }
}