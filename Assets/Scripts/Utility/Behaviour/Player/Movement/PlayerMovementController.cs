using UnityEngine;
using Utility.Manager.EventArgs;
using Utility.System.Publisher_Subscriber_System;

namespace Utility.Behaviour.Player.Movement
{
    public class PlayerMovementController : MonoBehaviour
    {
        private Subscription<InputEventArgs> inputEventSubscription;
        private Vector3 playerPosition;
        private Transform playerTransform;

        private void OnEnable()
        {
            playerTransform = transform;
            playerPosition = playerTransform.position;

            inputEventSubscription = PublisherSubscriber.Subscribe<InputEventArgs>(Move);
        }

        private void OnDisable()
        {
            PublisherSubscriber.Unsubscribe(inputEventSubscription);
        }

        public void SetInitialPosition(Vector3 position)
        {
            playerPosition = position;
        }

        private void Move(InputEventArgs inputEventArgs)
        {
            playerPosition += inputEventArgs.Delta;
            playerTransform.position = playerPosition;
        }
    }
}