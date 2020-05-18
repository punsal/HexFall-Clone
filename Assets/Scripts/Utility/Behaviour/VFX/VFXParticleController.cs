using EventArguments;
using UnityEngine;
using Utility.System.Publisher_Subscriber_System;

namespace Utility.Behaviour.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXParticleController : MonoBehaviour
    {
        [SerializeField] private GameEventType eventType = GameEventType.LevelComplete;

        private Subscription<GameEventType> genericEventTypeSubscription;

        private ParticleSystem system;

        private void OnEnable()
        {
            system = GetComponent<ParticleSystem>();

            genericEventTypeSubscription = PublisherSubscriber.Subscribe<GameEventType>(LevelCompleteEventHandler);
        }

        private void OnDisable()
        {
            PublisherSubscriber.Unsubscribe(genericEventTypeSubscription);
        }

        private void LevelCompleteEventHandler(GameEventType gameEventType)
        {
            if (gameEventType == eventType)
            {
                if (system.isPlaying) system.Stop();
                system.Play();
            }

            if (gameEventType == GameEventType.NextState) system.Stop();
        }
    }
}