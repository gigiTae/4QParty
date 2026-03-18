using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.AbilityCaster;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ServerCharacter : NetworkBehaviour
    {
        [SerializeField] ClientCharacter m_ClientCharacter;

        ServerAbilityCaster m_ServerAbilityCaster;


        public ClientCharacter ClientCharacter => m_ClientCharacter;


        [Rpc(SendTo.Server)]
        public void ServerCastAbilityRpc()
        {
            Debug.Log("ServerCastAbility");
        }


    }

}