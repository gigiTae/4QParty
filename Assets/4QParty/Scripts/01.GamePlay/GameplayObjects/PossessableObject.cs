using FQParty.GamePlay.Character;
using FQParty.GamePlay.Events;
using Unity.Netcode;
using UnityEngine;

namespace FQParty.GamePlay.GameplayObjects
{
    public class PossessableObject : NetworkBehaviour, IInteractable
    {
        public GameObject PossessObject => m_PossessObject;
        [SerializeField] GameObject m_PossessObject;

        [SerializeField] RequestPossessEvent m_RequestPossessEvent;

        public bool IsInteractable => m_IsInteractable;
        bool m_IsInteractable = true;

        public void Interact(ServerCharacter serverCharacter)
        {
            if (!IsServer || !m_IsInteractable) return;

            m_RequestPossessEvent.Raise(new RequestPossessContext()
            {
                PossessableObject = this,
                RequsetObject = serverCharacter
            });
            
            m_IsInteractable = false;
        }
    }
}