using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;
        public ClientCharacter ClientCharacter => m_ClientCharacter;




    }

}