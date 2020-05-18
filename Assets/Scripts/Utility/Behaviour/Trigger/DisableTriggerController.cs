using System;
using System.Collections;
using UnityEngine;

namespace Utility.Behaviour.Trigger
{
    public class DisableTriggerController : TriggerController
    {
        [SerializeField] protected DisableActionType actionType = DisableActionType.Immediately;
        [SerializeField] protected float timer;

        protected override void OnTriggerEnterAction(Collider other)
        {
            switch (actionType)
            {
                case DisableActionType.Immediately:
                    other.gameObject.SetActive(false);
                    break;
                case DisableActionType.Timed:
                    StartCoroutine(TimedDisable(other.gameObject));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator TimedDisable(GameObject objectToDisable)
        {
            var wait = new WaitForSeconds(timer);
            yield return wait;
            objectToDisable.SetActive(false);
        }

        protected enum DisableActionType
        {
            Immediately,
            Timed
        }
    }
}