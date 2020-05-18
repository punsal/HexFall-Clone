using System;
using UnityEngine;
using Utility.Behaviour.Turn.Type;
using Utility.Extension;

namespace Utility.Behaviour.Turn
{
    public class TurnAround : MonoBehaviour
    {
        [Header("Behaviour Attributes")]
        [SerializeField] private Axis turningAxis = Axis.Z;
        [SerializeField] private float speed = 10f;
        
        private Transform objectTransform;
        
        private void Awake()
        {
            objectTransform = transform;
        }

        private void Update()
        {
            Turn(turningAxis);
        }

        private void Turn(Axis axis)
        {
            var turningSpeed = Time.deltaTime * speed;
            switch (axis)
            {
                case Axis.X:
                    objectTransform.Rotate(Vector3.right * turningSpeed);
                    //objectTransform.eulerAngles = objectTransform.eulerAngles.AddX(turningSpeed);
                    break;
                case Axis.Y:
                    objectTransform.eulerAngles = objectTransform.eulerAngles.AddX(turningSpeed);
                    break;
                case Axis.Z:
                    objectTransform.eulerAngles = objectTransform.eulerAngles.AddX(turningSpeed);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}