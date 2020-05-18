using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utility.Behaviour.Camera.Data;
using Utility.Behaviour.Camera.Type;
using Utility.System.Publisher_Subscriber_System;

namespace Utility.Behaviour.Camera
{
    public class CameraTransformController : MonoBehaviour
    {
        private Subscription<string> cameraEventSubscription;

        private Coroutine coroutine;
        [SerializeField] private Transform mainCameraTransform;

        private void GetMainCamera()
        {
            if (mainCameraTransform != null) return;
            if (UnityEngine.Camera.main != null) mainCameraTransform = UnityEngine.Camera.main.transform;
        }

        public void SendCameraEventMessage(string message)
        {
            message = $"CAMERA_{message}";
            PublisherSubscriber.Publish(message);
        }

        private void OnCameraEventHandler(string message)
        {
            if (!message.Contains("CAMERA")) return;
            message = message.TrimStart("CAMERA_".ToCharArray());
            var index = 0;
            var isFound = false;
            for (var i = 0; i < cameraTransformData.Length; i++)
            {
                if (cameraTransformData[i].transformName != message) continue;
                isFound = true;
                index = i;
                break;
            }

            if (!isFound) return;

            if (coroutine != null) StopCoroutine(coroutine);

            switch (cameraTransformData[index].type)
            {
                case CameraTransformDataType.Once:
                    HandleOnce(index);
                    break;
                case CameraTransformDataType.Single:
                    HandleSingle(index);
                    break;
                case CameraTransformDataType.Multiple:
                    HandleMultiple(index);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void HandleOnce(int index)
        {
            var position = cameraTransformData[index].anchorTransform.position;
            var rotation = cameraTransformData[index].anchorTransform.eulerAngles;
            var flightDuration = cameraTransformData[index].flightDuration;

            mainCameraTransform.DOMove(position, flightDuration);
            mainCameraTransform.DORotate(rotation, flightDuration);
        }

        private void HandleSingle(int index)
        {
            HandleOnce(index);
            Invoke(nameof(ResetMainCameraTransform), cameraTransformData[index].returnDuration);
        }

        private void HandleMultiple(int index)
        {
            HandleOnce(index);
            coroutine = StartCoroutine(HandleStations(index));
        }

        private IEnumerator HandleStations(int index)
        {
            var currentCameraTransformData = cameraTransformData[index];
            yield return new WaitForSeconds(currentCameraTransformData.returnDuration);

            foreach (var data in currentCameraTransformData.stationData)
            {
                var currentStationPosition = data.station.position;
                var currentStationRotation = data.station.eulerAngles;

                mainCameraTransform.DOMove(currentStationPosition, data.flightDuration);
                mainCameraTransform.DORotate(currentStationRotation, data.flightDuration);

                yield return new WaitForSeconds(data.flightDuration + data.stayDuration);
            }

            ResetMainCameraTransform();
        }

        public void ResetMainCameraTransform()
        {
            var position = initialMainCameraTransform.position;
            var rotation = initialMainCameraTransform.eulerAngles;

            mainCameraTransform.DOMove(position, resetDuration);
            mainCameraTransform.DORotate(rotation, resetDuration);
        }
#pragma warning disable 649
        [SerializeField] private Transform initialMainCameraTransform;
        [SerializeField] private float resetDuration;
        [SerializeField] private CameraTransformData[] cameraTransformData;
#pragma warning restore 649

        #region UnityEngine Events

        private void OnValidate()
        {
            GetMainCamera();
        }

        private void Reset()
        {
            GetMainCamera();
        }

        private void Awake()
        {
            GetMainCamera();
        }

        private void OnEnable()
        {
            cameraEventSubscription = PublisherSubscriber.Subscribe<string>(OnCameraEventHandler);
        }

        private void OnDisable()
        {
            PublisherSubscriber.Unsubscribe(cameraEventSubscription);
        }

        #endregion
    }
}