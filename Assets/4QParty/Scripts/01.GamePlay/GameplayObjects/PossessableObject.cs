using FQParty.GamePlay.Character;
using UnityEngine;

namespace FQParty.GamePlay.GameplayObjects
{
    public class PossessableObject : MonoBehaviour, IInteractable
    {
        bool m_IsInteractable;

        [SerializeField] GameObject m_PossessObject; 

        public bool IsInteractable => m_IsInteractable;


        public void Interact(ServerCharacter serverCharacter)
        {
        
        }
        
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}