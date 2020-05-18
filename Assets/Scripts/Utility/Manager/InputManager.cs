using UnityEngine;
using UnityEngine.EventSystems;
using Utility.Manager.EventArgs;
using Utility.Manager.Type;
using Utility.System.Publisher_Subscriber_System;

namespace Utility.Manager
{
    public class InputManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private float animspeed;
        private Ray cameraRay;
        private Vector3 currentPositionVector;
#pragma warning disable 649
        [SerializeField] private LayerMask hitMask;
#pragma warning restore 649

        private Vector3 initialPositionVector;

        private InputEventArgs inputEventArgs;
        [SerializeField] private Camera mainCamera;
        private Vector3 positionDeltaVector;
        private RaycastHit raycastHit;

        public void OnDrag(PointerEventData eventData)
        {
            cameraRay = mainCamera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(cameraRay, out raycastHit, Mathf.Infinity, hitMask)) return;
            currentPositionVector = raycastHit.point;
            positionDeltaVector = currentPositionVector - initialPositionVector;
            initialPositionVector = currentPositionVector;

            inputEventArgs = new InputEventArgs
            {
                InputState = InputState.Drag,
                CurrentWorldPosition = currentPositionVector,
                Delta = positionDeltaVector,
                magnitude = eventData.delta.magnitude
            };
            PublisherSubscriber.Publish(inputEventArgs);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            cameraRay = mainCamera.ScreenPointToRay(eventData.position);
            if (!Physics.Raycast(cameraRay, out raycastHit, Mathf.Infinity, hitMask)) return;
            initialPositionVector = raycastHit.point;
            PublisherSubscriber.Publish(new InputEventArgs
            {
                InputState = InputState.Down,
                CurrentWorldPosition = initialPositionVector,
                Delta = Vector3.zero
            });
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PublisherSubscriber.Publish(new InputEventArgs {InputState = InputState.Up});
        }

        private void OnValidate()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }
    }
}