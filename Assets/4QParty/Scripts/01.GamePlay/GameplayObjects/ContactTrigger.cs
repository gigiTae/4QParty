using System;
using UnityEngine;

namespace FQParty.GamePlay.GameplayObjects
{
    public class ContactTrigger : MonoBehaviour
    {
        public Action<Collider> OnTriggerEneterEvent;

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEneterEvent?.Invoke(other);
        }

    }

}