using UnityEngine;

namespace Utility.Behaviour.Camera
{
    public class CameraSmoothFollow : MonoBehaviour
    {
        #pragma warning disable 649
        [SerializeField] private bool isLookingAt;
        #pragma warning restore 649
        [SerializeField] private Vector3 offset = Vector3.zero;

        [SerializeField] private float smoothSpeed = 0.125f;
#pragma warning disable 649
        [SerializeField] private Transform target;
#pragma warning restore 649
        
        private bool isTargetNull;

        private void Start()
        {
            isTargetNull = target == null;
        }

        private void FixedUpdate()
        {
            if (isTargetNull) return;
            var desiredPosition = target.position + offset;
            var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            if (isLookingAt) transform.LookAt(target);
        }

        public void CalculateOffset()
        {
            if (target == null) return;
            offset = transform.position - target.position;
        }
    }
}