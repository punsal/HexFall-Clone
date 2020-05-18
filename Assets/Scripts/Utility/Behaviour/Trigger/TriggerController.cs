using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utility.Behaviour.Trigger
{
    [RequireComponent(typeof(Collider))]
    public abstract class TriggerController : MonoBehaviour
    {
        [SerializeField] protected bool isAnyOther;
        [SerializeField] protected List<string> tags;
        private List<Collider> triggerColliders;

        private void OnValidate()
        {
            var colliders = GetComponents<Collider>();
            if (colliders == null || colliders.Length <= 0) return;
            triggerColliders = colliders.ToList();

            foreach (var triggerCollider in triggerColliders) triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isAnyOther)
            {
                OnTriggerEnterAction(other);
                return;
            }

            if (tags.Contains(other.tag)) OnTriggerEnterAction(other);
        }

        protected abstract void OnTriggerEnterAction(Collider other);
    }
}