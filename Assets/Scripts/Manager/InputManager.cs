using Item;
using UnityEngine;
using UnityEngine.EventSystems;
using Utility.Extension;
using Utility.System.Publisher_Subscriber_System;

namespace Manager
{
    public enum SwipeDirection
    {
        Left,
        Right
    }
    
    public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Screen View")]
        [SerializeField] private Camera cameraMain;
        #pragma warning disable 649
        [SerializeField] private LayerMask layerToHit;
        #pragma warning restore 649
        
        [Header("Screen Debug")]
        [SerializeField] private Vector2 initialScreenPosition;
        [SerializeField] private Vector2 currentScreenPosition;

        [Header("Scene Debug")]
        [SerializeField] private Vector3 initialPosition;
        [SerializeField] private Vector3 currentPosition;

        private void OnValidate()
        {
            if (cameraMain == null)
            {
                cameraMain = Camera.main;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            initialScreenPosition = eventData.position;
            initialPosition = cameraMain.GetWorldPositionIn3D(initialScreenPosition, -1f * cameraMain.transform.position.z);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            currentScreenPosition = eventData.position;
            currentPosition = cameraMain.GetWorldPositionIn3D(currentScreenPosition, -1f * cameraMain.transform.position.z);

            var distance = Vector3.Distance(initialPosition, currentPosition);
            if (distance >= 1f)
            {
                //Try to Swipe
                if (initialScreenPosition.x > currentScreenPosition.x)
                {
                    PublisherSubscriber.Publish(SwipeDirection.Left);
                }
                else
                {
                    PublisherSubscriber.Publish(SwipeDirection.Right);
                }
                return;
            }

            var hit = Physics2D.Raycast(initialPosition, cameraMain.transform.forward, Mathf.Infinity, layerToHit);
            if (hit.transform == null) return;
            if (hit.transform.GetComponent<GridItem>(out var gridItem))
            {
                gridItem.Select(hit.point);
            }
        }
    }
}
