using FQParty.GamePlay.Abilities;
using FQParty.GamePlay.Abilities.Effects;
using Unity.Netcode;
using UnityEngine;


namespace FQParty.GamePlay.Character
{
    public class ClientCharacter : NetworkBehaviour
    {
        [SerializeField] ServerCharacter m_ServerCharacter;
        [SerializeField] ClientCharacterMovement m_ClientCharacterMovement;
        [SerializeField] AbilityDatabase m_Database;

    }

}